using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private int initialSnakeSize;
    
        [Space(10)][Header("Player Information")]
        [Tooltip("Mark if it's not AI")][SerializeField] protected bool isPlayer;
        
        [Space(10)][Header("AI Information - No need to fill in if it's not AI.")]
        [SerializeField] private BoxCollider2D pointLeft;
        [SerializeField] private BoxCollider2D pointRight;
        [SerializeField] private BoxCollider2D pointFrontLeft;
        [SerializeField] private BoxCollider2D pointFrontRight;
        [SerializeField] private BoxCollider2D pointBackLeft;
        [SerializeField] private BoxCollider2D pointBackRight;

        [Space(10)][Header("Essentials")]
        [SerializeField] private BoxCollider2D pointFront;
        [SerializeField] private GameObject bodyPartPrefab;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask wallsLayer;
        [SerializeField] private BlockManager blockManager;
    
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
        private readonly List<Transform> _bodyParts = new List<Transform>();
        private Vector3 _direction;
        private Directions _currentDir = Directions.Right;
        private bool _canMove = true;
        
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
            
            if (isPlayer) 
            {
                GameManager.Instance.ResetGame();
                SetUpControls();
                Controls.Enable();
            }
            
            ChangeDirection(blockManager.GetLastGeneratedBlockPosition());
            StartCoroutine(Move());
        }

        private void ResetSnake()
        {
            ResetBodyParts();
            snakeVariables.OnResetSnake();
            SetRandomRotation();
            transform.position = blockManager.GetRandomPosition(false);
            SetUpInitialSize();
            ChangeDirection(blockManager.GetLastGeneratedBlockPosition());
        }

        private void SetRandomRotation()
        {
            float chance = Random.Range(0, 1f);
            
            if (chance > 0.75f)
            {
                Turn(Directions.Left);
            }
            else if (chance > 0.5f)
            {
                Turn(Directions.Right);
            }
            else if (chance > 0.25f)
            {
                Turn(Directions.Up);
            }
            else
            {
                Turn(Directions.Down);
            }
        }

        private void ResetBodyParts()
        {
            foreach (var part in _bodyParts.Where(part => part != transform))
            {
                Destroy(part.gameObject);
            }
            _bodyParts.Clear();
            _bodyParts.Add(transform);
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
                if (GameManager.Instance.IsGameOver()) yield break;
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

        private void Turn(Directions dir)
        {
            if (!_canMove) return;
        
            _canMove = false;
 
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
            bool isTouchingObstacle = pointFront.IsTouchingLayers(obstacleLayer);
            bool isTouchingWalls = pointFront.IsTouchingLayers(wallsLayer);

            if (!isTouchingObstacle && !isTouchingWalls) return false;
            
            if (isTouchingObstacle)
            {
                bool success = snakePowerUp.TryUseBatteringRam();

                if (success)
                {
                    DecreaseSize();
                    return false;
                }
            }

            if (!isPlayer)
            {
                RespawnAI();
                return false;
            }
            
            GameOver();
            return true;
        }

        private void GameOver()
        {
            if (isPlayer)
            {
                GameManager.Instance.EndGame();
                Controls.Disable();
            }
            else
            {
                ResetSnake();
            }
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
        private void MoveAI()
        {
            if (pointFront.IsTouchingLayers(obstacleLayer)) TurnToDesiredDirection(_currentDir + 2);
            
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

        private void RespawnAI()
        {
            ResetSnake();
        }
        
        private void TurnToDesiredDirection(Directions desired)
        {
            if (_currentDir == desired) return;
            if (!_canMove) return;

            bool avoidObstacleLeft = pointLeft.IsTouchingLayers(obstacleLayer);
            bool avoidObstacleRight = pointRight.IsTouchingLayers(obstacleLayer);

            var value = _currentDir - desired;

            switch (value)
            {
                case 1:
                case -3:
                    if (avoidObstacleLeft) return;
                    Turn(GetLeftDirection());
                    break;
                case -2:
                case 2:
                    switch (avoidObstacleLeft)
                    {
                        case true when avoidObstacleRight:
                            return;
                        
                        case true:
                            Turn(GetRightDirection());
                            break;
                        
                        case false when avoidObstacleRight:
                            Turn(GetLeftDirection());
                            break;
                        case false:
                            Turn(GetCorrectDirection());
                            break;
                    }
                    break;
                case -1:
                case 3:
                    if (avoidObstacleRight) return;
                    Turn(GetRightDirection());
                    break;
            }
        }

        private Directions GetCorrectDirection()
        {
            int leftSum = 0;
            int rightSum = 0;
            
            if (pointFrontLeft.IsTouchingLayers(obstacleLayer)) leftSum++;
            if (pointFrontRight.IsTouchingLayers(obstacleLayer)) leftSum++;
            if (pointBackLeft.IsTouchingLayers(obstacleLayer)) rightSum++;
            if (pointBackRight.IsTouchingLayers(obstacleLayer)) rightSum++;

            return leftSum > rightSum ? GetLeftDirection() : GetRightDirection();
        }
        
        private Directions GetRightDirection()
        {
            return _currentDir == Directions.Down ? Directions.Left : _currentDir + 1;
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
