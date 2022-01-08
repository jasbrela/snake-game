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
        [SerializeField] private SnakeVariables snakeVariables;
        [SerializeField] private SnakePowerUp snakePowerUp;
        [SerializeField] private int initialSnakeSize;
        [Tooltip("The order matters.")][SerializeField] private Transform[] spawnPoints;

        [Space(10)][Header("Player Information")]
        [Tooltip("Mark if it's not AI")][SerializeField] protected bool isPlayer;
        
        [Space(10)][Header("AI Information - No need to fill in if it's not AI.")]
        [SerializeField] private BoxCollider2D pointLeft;
        [SerializeField] private BoxCollider2D pointRight;
        [SerializeField] private BoxCollider2D pointFrontLeft;
        [SerializeField] private BoxCollider2D pointFrontRight;
        [SerializeField] private BoxCollider2D pointBackLeft;
        [SerializeField] private BoxCollider2D pointBackRight;

        [Space(10)] [Header("Essentials")]
        [SerializeField] private BoxCollider2D pointFront;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask wallsLayer;
        [SerializeField] private BlockManager blockManager;
        
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
            GameManager.Instance.SetMaxSpawnPoints(spawnPoints.Length-1);
            GameManager.Instance.SendOnPressRetryCallback(ResetSnake);
            GetComponent<SpriteRenderer>().color = color;
            
            if (isPlayer) return;
            blockManager.SendOnGeneratedRandomPositionCallback(ChangeDirection);
        }

        private void Start()
        {
            if (isPlayer) 
            {
                SetUpControls();
            }
            
            if (!isPlayer) ChangeDirection(blockManager.GetLastGeneratedBlockPosition());
            
            StartCoroutine(Move());
        }

        private void ResetSnake()
        {
            snakePowerUp.ResetPowerUps();
            ResetBodyParts();
            snakeVariables.OnResetSnake();
            SetPosition(GameManager.Instance.GetNextSpawnPoint());
            SetUpInitialSize();
            ChangeDirection(blockManager.GetLastGeneratedBlockPosition());
            
            if (isPlayer) Controls.Enable();
        }

        private void SetPosition(int index)
        {
            transform.position = spawnPoints[index].position;
            Directions first = spawnPoints[index].position.x > 0 ? Directions.Left : Directions.Right;
            Directions second = spawnPoints[index].position.y > 0 ? Directions.Down : Directions.Up;
            
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
                
                if (!isPlayer) TurnAI();
                if (HasCollided()) yield return null;
                while (GameManager.Instance.IsGameOver()) yield return null;
                ChangeSnakePosition();
                
                yield return new WaitForSeconds(snakeVariables.Speed);
                
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

        private void Turn(Directions dir)
        {
            if (!_canTurn) return;
        
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
                ResetSnake();
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
                if (block.Type == PowerUp.EnginePower) snakeVariables.OnPickupEnginePowerBlock();
                IncreaseSize();
            }
        }

        private void IncreaseSize()
        {
            snakeVariables.OnPickupAnyBlock();
            Transform reference = _bodyParts[_bodyParts.Count - 1];
        
            GameObject block = Instantiate(bodyPartPrefab, reference.position - reference.right, reference.rotation);
            block.GetComponent<SpriteRenderer>().color = color;
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
        private void TurnAI()
        {
            if (pointFront.IsTouchingLayers(obstacleLayer) ||
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
