using System.Collections;
using System.Collections.Generic;
using Blocks;
using Enums;
using UnityEngine;

namespace Snake
{
    public class Snake : MonoBehaviour
    {
        #region Variables
        [Header("Snake Information")]
        
        [SerializeField] private SnakeVariables snakeVariables;
        [SerializeField] private SnakePowerUp snakePowerUp;
    
        [Space(10)][Header("Player Information")]
        [SerializeField] protected bool isPlayer;
    
        [Space(10)][Header("Essentials")]
        [SerializeField] private BlockManager blockManager;
        [SerializeField] private GameObject bodyPartPrefab;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Transform predictionPoint;
        [SerializeField] private int initialSnakeSize;
    
        private Vector3 _direction;
        private Controls _controls;
        private Controls Controls
        {
            get
            {
                if (_controls != null)
                {
                    return _controls;
                }

                return _controls = new Controls();
            }
        }

        private Directions _currentDir = Directions.Right;
        private bool _canMove = true;
        private readonly List<Transform> _bodyParts = new List<Transform>();
        
        #endregion
        
        private void Awake()
        {
            if (isPlayer) return;
            enabled = true;
            blockManager.SendOnGeneratedRandomPositionCallback(ChangeDirection);
        }

        private void Start()
        {
            ResetSnake();
            if (isPlayer) Controls.Enable();
            _direction = blockManager.GetLastGeneratedBlockPosition();
            StartCoroutine(Move());
        }

        private void ResetSnake()
        {
            transform.rotation = Quaternion.identity;
            SetUpControls();
            _bodyParts.Clear();
            _bodyParts.Add(transform);
            SetUpInitialSize();
        }

        private void SetUpInitialSize()
        {
            for (var i = 0; i < initialSnakeSize; i++)
            {
                IncreaseSize();
            }
        }

        private void SetUpControls()
        {
            Controls.Player.TurnLeft.performed += _ => Turn(Directions.Left);
            Controls.Player.TurnRight.performed += _ => Turn(Directions.Right);
            Controls.Player.TurnUp.performed += _ => Turn(Directions.Up);
            Controls.Player.TurnDown.performed += _ => Turn(Directions.Down);
        }

        private IEnumerator Move()
        {
            while (true)
            {
                if (HasCollided()) yield break;
                if (!isPlayer) MoveAI();
                MoveSnake();

                yield return new WaitForSeconds(snakeVariables.Speed);
                _canMove = true;
            }
        }

        private void MoveSnake()
        {
            for (int i = _bodyParts.Count - 1; i > 0; i--)
            {
                _bodyParts[i].rotation = _bodyParts[i - 1].rotation;
                _bodyParts[i].position = _bodyParts[i - 1].position;
            }

            Transform t = transform;
            t.position += t.right;
        }

        private void MoveAI()
        {
            var pos = transform.position;
            var relDir = pos - _direction;
            
            if (Mathf.Abs(relDir.x) > Mathf.Abs(relDir.y))
            {
                TurnToDesiredDirection(_direction.x > pos.x ? Directions.Right : Directions.Left);
            }
            else
            {
                TurnToDesiredDirection(_direction.y > pos.y ? Directions.Up : Directions.Down);
            }
        }
        
        private void Turn(Directions dir)
        {
            if (!_canMove) return;
        
            _canMove = false;
 
            Debug.Log("Turn to " + dir);
            var z = transform.rotation.z;
            switch (dir)
            {
                case Directions.Left:
                    if (_currentDir == Directions.Right) return;
                    _currentDir = Directions.Left;
                    z = 180;
                    break;
                case Directions.Right:
                    if (_currentDir == Directions.Left) return;
                    _currentDir = Directions.Right;
                    z = 0;
                    break;
                case Directions.Up:
                    if (_currentDir == Directions.Down) return;
                    _currentDir = Directions.Up;
                    z = 90;
                    break;
                case Directions.Down:
                    if (_currentDir == Directions.Up) return;
                    _currentDir = Directions.Down;
                    z = -90;
                    break;
            }
        
            transform.rotation = Quaternion.Euler(0, 0, z);
        }

        private bool HasCollided()
        {
            if (!predictionPoint.GetComponent<BoxCollider2D>().IsTouchingLayers(obstacleLayer)) return false;
            
            bool success = snakePowerUp.TryUseBatteringRam();
            
            if (success)
            {
                DecreaseSize();
            }
            else
            {
                GameOver();
                return true;
            }

            return false;
        }

        private void GameOver()
        {
            ResetSnake();
            if (!isPlayer) return;
            Controls.Disable();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Block"))
            {
                Block block = other.GetComponent<Block>();
                if (block.Type == PowerUp.EnergyEngine) snakeVariables.OnPickupEnergyEngineBlock();
                IncreaseSize();
            }
        }

        private void IncreaseSize()
        {
            snakeVariables.OnPickupAnyBlock();
            Transform reference = _bodyParts[_bodyParts.Count - 1];
        
            GameObject block = Instantiate(bodyPartPrefab, reference.position - reference.right, reference.rotation);
            _bodyParts.Add(block.transform);
            block.transform.parent = transform.parent;
        }

        private void DecreaseSize()
        {
            snakeVariables.OnUseBatteringRam();
            Destroy(_bodyParts[_bodyParts.Count - 1].gameObject);
            _bodyParts.RemoveAt(_bodyParts.Count - 1);
        }

        #region AI Part
        private void TurnToDesiredDirection(Directions desired)
        {
            if (_currentDir == desired) return;
            if (!_canMove) return;
            
            var value = _currentDir - desired;

            switch (value)
            {
                case 1:
                case -3:
                    Turn(GetLeftDirection());
                    break;
                case -2:        // this case is whether right or left.
                case 2:         // TODO: so, add more checkups before deciding turn
                    Turn(GetLeftDirection());
                    break;
                case -1:
                case 3:
                    Turn(GetRightDirection());
                    break;
            }
        }

        private Directions GetRightDirection()
        {
            return _currentDir == Directions.Down ? Directions.Left : _currentDir + 1;;
        }
        
        private Directions GetLeftDirection()
        {
            
            return _currentDir == Directions.Left ? Directions.Down : _currentDir - 1;
        }
        
        private void ChangeDirection(Vector3 dir)
        {
            _direction = dir;
        }
        
        #endregion
    }
}
