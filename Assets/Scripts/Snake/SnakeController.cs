using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Snake
{
    public class SnakeController : MonoBehaviour
    {
        #region Variables

        [Header("Snake Information")]
        [SerializeField] private Color color;
        [SerializeField] private GameObject bodyPartPrefab;
        [SerializeField] private SnakeWeight snakeWeight;
        [SerializeField] private SnakePowerUp snakePowerUp;
        [SerializeField] private int initialSnakeSize;

        [Space(10)][Header("AI Information - AIHandler can be null")]
        [Tooltip("Mark if it's not AI")]
        [SerializeField] protected bool isAI;
        [SerializeField] private AIHandler aiHandler;

        [Space(10)] [Header("Essentials")]
        [SerializeField] private BoxCollider2D pointFront;
        [Tooltip("This layer allow the snake to use its battering ram blocks before dying")]
        [SerializeField] private LayerMask obstaclesLayer;
        [Tooltip("This layer will kill the snake instantly")]
        [SerializeField] private LayerMask wallsLayer;
        // PLAYER
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

        // AI
        private Vector3 _direction;
        
        // BOTH
        private readonly List<Transform> _bodyParts = new List<Transform>();
        private Directions _currentDir = Directions.Right;
        private bool _canTurn = true;
        private bool _collided;
        #endregion
        
        private void Awake()
        {
            GameManager.Instance.SendOnPressRetryCallback(ResetSnake);
            GetComponent<SpriteRenderer>().color = color;
        }

        private void Start()
        {
            if (!isAI) 
            {
                SetUpControls();
            }

            if (isAI)
            {
                BlockManager.Instance.SendOnGeneratedRandomPositionCallback(ChangeDirection);
                ChangeDirection(BlockManager.Instance.GetLastGeneratedBlockPosition());
            }
            
            StartCoroutine(Move());
        }

        private void ResetSnake()
        {
            snakePowerUp.ResetPowerUps();
            ResetBodyParts();
            snakeWeight.OnResetSnake();
            SetPosition();
            SetUpInitialSize();
            ChangeDirection(BlockManager.Instance.GetLastGeneratedBlockPosition());
            
            if (!isAI) Controls.Enable();
        }

        private void SetPosition()
        {
            Vector3 spawnPoint = GameManager.Instance.GetNextSpawnPoint().position;
            
            transform.position = spawnPoint;
            
            Directions first = spawnPoint.x > 0 ? Directions.Left : Directions.Right;
            Directions second = spawnPoint.y > 0 ? Directions.Down : Directions.Up;
            
            SetRandomRotation(first, second);
        }

        private void SetRandomRotation(Directions one, Directions two)
        {
            float chance = Random.Range(0, 1f);
            Turn(chance > 0.5f ? one : two);
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
                // BUG: AI is dying as soon as it touches wall without any chance to turn.
                
                if (isAI) TurnAI();
                if (HasCollided()) yield return null;
                while (GameManager.Instance.IsGameOver()) yield return null;
                ChangeSnakePosition();
                
                yield return new WaitForSeconds(snakeWeight.Speed);
                
                _canTurn = true;
            }
        }

        private void ChangeSnakePosition()
        {
            for (int i = _bodyParts.Count - 1; i > 0; i--)
            {
                _bodyParts[i].rotation = _bodyParts[i - 1].rotation;
                _bodyParts[i].position = _bodyParts[i - 1].position;
            }

            Transform t = transform;
            t.position += t.right;
        }

        private void Turn(Directions? dir)
        {
            if (!_canTurn || dir == null) return;
        
            _canTurn = false;
 
            float z = transform.rotation.z;
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
            
            HasCollided();
        }

        private bool HasCollided()
        {
            bool isTouchingObstacle = pointFront.IsTouchingLayers(obstaclesLayer);
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

            if (isAI)
            {
                ResetSnake();
                return false;
            }
            
            GameOver();
            return true;
        }

        private void GameOver()
        {
            if (!isAI)
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
                BlockController blockController = other.GetComponent<BlockController>();
                if (blockController.Type == PowerUp.EnginePower) snakeWeight.OnPickupEnginePowerBlock();
                IncreaseSize();
            }
        }

        private void IncreaseSize()
        {
            snakeWeight.OnPickupAnyBlock();
            Transform reference = _bodyParts[_bodyParts.Count - 1];
        
            GameObject block = Instantiate(bodyPartPrefab, reference.position - reference.right, reference.rotation);
            block.GetComponent<SpriteRenderer>().color = color;
            _bodyParts.Add(block.transform);
            block.transform.parent = transform.parent;
        }

        private void DecreaseSize()
        {
            snakeWeight.OnUseBatteringRam();
            Destroy(_bodyParts[_bodyParts.Count - 1].gameObject);
            _bodyParts.RemoveAt(_bodyParts.Count - 1);
        }

        #region AI Part
        private void TurnAI()
        {
            if (pointFront.IsTouchingLayers(obstaclesLayer) ||
                pointFront.IsTouchingLayers(wallsLayer)) TurnToDesiredDirection(_currentDir + 2);
            
            Vector3 pos = transform.position;
            Vector3 relativePosition = pos - _direction;
            
            if (Mathf.Abs(relativePosition.x) > Mathf.Abs(relativePosition.y))
            {
                TurnToDesiredDirection(_direction.x > pos.x ? Directions.Right : Directions.Left);
            }
            else
            {
                TurnToDesiredDirection(_direction.y > pos.y ? Directions.Up : Directions.Down);
            }
        }

        private void TurnToDesiredDirection(Directions desired)
        {
            if (_currentDir == desired) return;
            if (!_canTurn) return;

            bool avoidLeft = aiHandler.IsLeftPointColliding();
            bool avoidRight = aiHandler.IsRightPointColliding();

            var value = _currentDir - desired;

            switch (value)
            {
                case 1:
                case -3:
                    if (avoidLeft) return;
                    Turn(GetLeftDirection());
                    break;
                case -2:
                case 2:
                    switch (avoidLeft)
                    {
                        case true when avoidRight:
                            return;
                        
                        case true:
                            Turn(GetRightDirection());
                            break;
                        
                        case false when avoidRight:
                            Turn(GetLeftDirection());
                            break;
                        case false:
                            Turn(GetCorrectDirection());
                            break;
                    }
                    break;
                case -1:
                case 3:
                    if (avoidRight) return;
                    Turn(GetRightDirection());
                    break;
            }
        }

        private Directions? GetCorrectDirection()
        {
            Directions side = aiHandler.GetTheFreerSide();
            switch (side)
            {
                case Directions.Left:
                    return GetLeftDirection();
                case Directions.Right:
                    return GetRightDirection();
            }
            return null;
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
