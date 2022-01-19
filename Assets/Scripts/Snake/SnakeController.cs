using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Snake
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Collider2D))]
    public class SnakeController : MonoBehaviour
    {
        #region Variables

        [Header("Snake Information")]
        [SerializeField] private Color color;
        [SerializeField] private GameObject bodyPartPrefab;
        [SerializeField] private SnakeWeight snakeWeight;
        [SerializeField] private SnakePowerUp snakePowerUp;
        [SerializeField] private int initialSnakeSize;

        [Header("AI Information - AIHandler can be null if it's not AI")]
        [Tooltip("Mark if it's not AI")]
        [SerializeField] protected bool isAI;
        [SerializeField] private AIHandler aiHandler;

        [Header("Essentials")]
        [SerializeField] private BoxCollider2D pointFront;
        [Tooltip("This layer allow the snake to use its battering ram blocks before dying")]
        [SerializeField] private LayerMask obstaclesLayer;
        [Tooltip("This layer will kill the snake instantly")]
        [SerializeField] private LayerMask wallsLayer;

        // PLAYER
        private PlayerInput _input;
        private SnakeManager manager;

        // AI
        private Vector3 _direction;
        
        // BOTH
        private SpriteRenderer _headSprite;
        private readonly List<Transform> _bodyParts = new List<Transform>();
        private Directions _currentDir = Directions.Right;
        private bool _canTurn = true;
        private bool _canTurnDrastically;
        private bool _collided;
        
        public delegate void OnPlayerSnakeDie(SnakeManager manager);
        private OnPlayerSnakeDie onPlayerSnakeDieCallback;
        
        public delegate void OnAISnakeDie(GameObject parent);
        private OnAISnakeDie onAISnakeDieCallback;
        #endregion

        /// <summary>
        /// Used to send the callback to be called when the player dies.
        /// </summary>
        /// <param name="onPlayerSnakeDie">A void method to be called.</param>
        public void SendOnPlayerSnakeDieCallback(OnPlayerSnakeDie onPlayerSnakeDie)
        {
            if (!isAI) onPlayerSnakeDieCallback = onPlayerSnakeDie;
        }
        
        /// <summary>
        /// Used to send the callback to be called when the AI dies.
        /// </summary>
        /// <param name="onAISnakeDie">A void method to be called.</param>
        public void SendOnAISnakeDieCallback(OnAISnakeDie onAISnakeDie)
        {
            if (isAI) onAISnakeDieCallback = onAISnakeDie;
        }
        
        /// <summary>
        /// Sets the snake's color
        /// </summary>
        /// <param name="color">A color to be the snake's color.</param>
        public void SetColor(Color color)
        {
            this.color = color;
            _headSprite.color = color;
        }
        
        private void OnEnable()
        {
            if (!isAI) _input = GetComponent<PlayerInput>();
            GameManager.Instance.SendOnPressRetryCallback(ResetSnake);
            _headSprite = GetComponent<SpriteRenderer>();
            
            GameManager.Instance.SendOnGameStartsForTheFirstTimeCallback(StartGame);
            
            if (!isAI)
            {
                SetUpControls();
            }

            if (GameManager.IsAMultiplayerGame() && !isAI)
            {
                manager = transform.parent.GetComponent<SnakeManager>();
                color = manager.Info.Color;
            }
            
            if (!isAI) return;
            BlockManager.Instance.SendOnGeneratedRandomPositionCallback(ChangeDirection);
            ChangeDirection(BlockManager.Instance.GetLastGeneratedBlockPosition());
            
        }

        private void Start()
        {
            _headSprite.color = color;
        }

        /// <summary>
        /// On game starts, start the Move Coroutine
        /// </summary>
        private void StartGame()
        {
            StartCoroutine(Move());
        }

        /// <summary>
        /// Prepares the snake to spawn again.
        /// </summary>
        public void ResetSnake()
        {
            _canTurnDrastically = true;
            snakePowerUp.ResetPowerUps();
            ResetBodyParts();
            snakeWeight.OnResetSnake();
            
            SetSnakeForSpawn();
            
            if (GameManager.IsAMultiplayerGame() && !isAI)
            {
                snakePowerUp.AddInitialPowerUps(manager.currentPreset);
            }

            ChangeDirection(BlockManager.Instance.GetLastGeneratedBlockPosition());
            
            if (!isAI) _input.actions.Enable();
        }

        /// <summary>
        /// Set up both position and rotation for the snake that is going to be spawned.
        /// </summary>
        private void SetSnakeForSpawn()
        {
            Vector3 spawnPoint = GameManager.Instance.GetNextSpawnPointPosition();
            
            if (GameManager.IsAMultiplayerGame())
            {
                _canTurn = true;
                Turn(spawnPoint.x > 0 ? Directions.Left : Directions.Right);
            }
            else
            {
                Directions first = spawnPoint.x > 0 ? Directions.Left : Directions.Right;
                Directions second = spawnPoint.y > 0 ? Directions.Down : Directions.Up;

                SetRandomRotation(first, second);
            }

            transform.position = spawnPoint;
            _headSprite.enabled = true;
            SetUpInitialSize();
        }

        /// <summary>
        /// Picks a random rotation between two possibilities.
        /// </summary>
        /// <param name="one">First Possible direction</param>
        /// <param name="two">Second Possible direction</param>
        private void SetRandomRotation(Directions one, Directions two)
        {
            float chance = Random.Range(0, 1f);
            Turn(chance > 0.5f ? one : two);
        }

        /// <summary>
        /// Resets the snake body then prepares it for respawn.
        /// </summary>
        private void ResetBodyParts()
        {
            foreach (var part in _bodyParts.Where(part => part != transform))
            {
                Destroy(part.gameObject);
            }
            _bodyParts.Clear();
            _bodyParts.Add(transform);
            _headSprite.enabled = false;
        }

        /// <summary>
        /// Spawn the body parts needed to set the defined initial's snake size.
        /// </summary>
        private void SetUpInitialSize()
        {
            for (var i = 0; i < initialSnakeSize; i++)
            {
                IncreaseSize();
            }
        }

        /// <summary>
        /// Set up the controls for human players.
        /// </summary>
        private void SetUpControls()
        {
            _input.actions[InputActions.TurnLeft.ToString()].performed += _ => Turn(GetLeftDirection());
            _input.actions[InputActions.TurnRight.ToString()].performed += _ => Turn(GetRightDirection());
        }

        /// <summary>
        /// Handles the movement cycle.
        /// </summary>
        private IEnumerator Move()
        {
            while (true)
            {
                // BUG: AI is dying as soon as it touches wall directly without any chance to turn.

                if (isAI) TurnAI();
                if (HasCollided()) yield return null;
                while (GameManager.Instance.IsGameOver()) yield return null;
                MoveSnakeBody();
                
                yield return new WaitForSeconds(snakeWeight.Speed);
                
                _canTurn = true;
            }
        }

        /// <summary>
        /// Move the body parts following the snake's head movement.
        /// </summary>
        private void MoveSnakeBody()
        {
            for (int i = _bodyParts.Count - 1; i > 0; i--)
            {
                _bodyParts[i].rotation = _bodyParts[i - 1].rotation;
                _bodyParts[i].position = _bodyParts[i - 1].position;
            }

            Transform t = transform;
            t.position += t.right;
        }

        /// <summary>
        /// Rotates the snake's head left or right.
        /// </summary>
        /// <param name="dir">Desired direction</param>
        private void Turn(Directions? dir)
        {
            if (!_canTurn || dir == null) return;
        
            _canTurn = false;
 
            float z = transform.rotation.z;
            switch (dir)
            {
                case Directions.Left:
                    if (!_canTurnDrastically && _currentDir == Directions.Right) return;
                    _currentDir = Directions.Left;
                    z = 180;
                    break;
                case Directions.Right:
                    if (!_canTurnDrastically && _currentDir == Directions.Left) return;
                    _currentDir = Directions.Right;
                    z = 0;
                    break;
                case Directions.Up:
                    if (!_canTurnDrastically && _currentDir == Directions.Down) return;
                    _currentDir = Directions.Up;
                    z = 90;
                    break;
                case Directions.Down:
                    if (!_canTurnDrastically && _currentDir == Directions.Up) return;
                    _currentDir = Directions.Down;
                    z = -90;
                    break;
            }

            if (_canTurnDrastically) _canTurnDrastically = false;
        
            transform.rotation = Quaternion.Euler(0, 0, z);
        }
        
        /// <summary>
        /// Checks if the player has collided with something, then handle the result.
        /// </summary>
        /// <returns>A bool regarding if the player has collided</returns>
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

            if (GameManager.IsAMultiplayerGame())
            {
                if (isAI)
                {
                    onAISnakeDieCallback?.Invoke(transform.parent.gameObject);
                }
                else
                {
                    onPlayerSnakeDieCallback?.Invoke(manager);
                }
                return true;
            }

            if (isAI)
            {
                ResetSnake();
                return false;
            }
            
            GameOver();
            return true;
        }

        /// <summary>
        /// Ends the game and disable Player's controls.
        /// </summary>
        private void GameOver()
        {
            if (isAI) return;
            
            GameManager.Instance.EndGame();
            _input.actions.Disable();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Block") || GameManager.Instance.IsGameOver()) return;
            
            BlockController blockController = other.GetComponent<BlockController>();
            if (blockController.Type == PowerUp.EnginePower) snakeWeight.OnPickupEnginePowerBlock();
            IncreaseSize();
        }

        /// <summary>
        /// Increase the snake's size and weight.
        /// </summary>
        private void IncreaseSize()
        {
            snakeWeight.OnPickupAnyBlock();
            Transform reference = _bodyParts[_bodyParts.Count - 1];
        
            GameObject bodyPart = Instantiate(bodyPartPrefab, reference.position - reference.right, reference.rotation);
            bodyPart.GetComponent<SpriteRenderer>().color = color;
            _bodyParts.Add(bodyPart.transform);
            bodyPart.transform.parent = transform.parent;
        }

        /// <summary>
        /// Decrease the snake's size and weight.
        /// </summary>
        private void DecreaseSize()
        {
            snakeWeight.OnUseBatteringRam();
            Destroy(_bodyParts[_bodyParts.Count - 1].gameObject);
            _bodyParts.RemoveAt(_bodyParts.Count - 1);
        }

        #region AI Part
        /// <summary>
        /// Handles the AI's needs to turn.
        /// </summary>
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

        /// <summary>
        /// Based on a desired location, turn the snake to the nearest side.
        /// </summary>
        /// <param name="desired">Desired direction</param>
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

        /// <summary>
        /// If it does not matter if it's going to turn to left or right,
        /// this will return the freer side to turn.
        /// </summary>
        /// <returns>A direction to the freer side.</returns>
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
        
        /// <summary>
        /// This will get the right direction related to the current direction.
        /// </summary>
        /// <returns>The right direction of the current direction.</returns>
        private Directions GetRightDirection()
        {
            return _currentDir == Directions.Down ? Directions.Left : _currentDir + 1;
        }
        
        /// <summary>
        /// This will get the left direction related to the current direction.
        /// </summary>
        /// <returns>The left direction of the current direction.</returns>
        private Directions GetLeftDirection()
        {
            return _currentDir == Directions.Left ? Directions.Down : _currentDir - 1;
        }
        
        /// <summary>
        /// Changes the target direction for the AI snake.
        /// </summary>
        /// <param name="target">Target direction</param>
        private void ChangeDirection(Vector3 target)
        {
            _direction = target;
        }
        
        #endregion
    }
}
