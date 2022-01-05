using System;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Snake Information")]
    [SerializeField] private SnakeVariables snakeVariables;
    [SerializeField] private SnakePowerUp snakePowerUp;
    
    [Space(10)][Header("Player Information")]
    [SerializeField] private bool isPlayer;
    
    [Space(10)][Header("Essentials")]
    [SerializeField] private GameObject bodyPartPrefab;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform predictionPoint;
    [SerializeField] private int initialSnakeSize;
    
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

    private bool _canMove = true;
    private readonly List<Transform> _bodyParts = new List<Transform>();

    void Start()
    {
        ResetSnake();
        if (isPlayer) Controls.Enable();
        Invoke(nameof(Move), 0); // TODO: change to coroutines
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

    void Move()
    {
        if (predictionPoint.GetComponent<BoxCollider2D>().IsTouchingLayers(obstacleLayer))
        {
            bool success = snakePowerUp.TryUseBatteringRam();
            if (success)
            {
                DecreaseSize();
            } else
            {
                GameOver();
                return;
            }
        }

        for (int i = _bodyParts.Count - 1; i > 0; i--)
        {
            _bodyParts[i].rotation = _bodyParts[i - 1].rotation;
            _bodyParts[i].position = _bodyParts[i - 1].position;
        }

        Transform t = transform;
        t.position += t.right;
        
        Invoke(nameof(Move), snakeVariables.Speed);
        _canMove = true;
    }

    private void GameOver()
    {
        if (isPlayer) Controls.Disable();
    }
    
    private void Turn(Directions dir)
    {
        if (!_canMove) return;
        
        _canMove = false;
        
        var z = transform.rotation.z;
        switch (dir)
        {
            case Directions.Left:
                if (transform.rotation.eulerAngles.z == 0) return;
                z = 180;
                break;
            case Directions.Right:
                if (Math.Abs(transform.rotation.eulerAngles.z - (-180)) < 1) return;
                z = 0;
                break;
            case Directions.Up:
                if (Math.Abs(transform.rotation.eulerAngles.z - (-90)) < 1) return;
                z = 90;
                break;
            case Directions.Down:
                if (Math.Abs(transform.rotation.eulerAngles.z - 90) < 1) return;
                z = -90;
                break;
        }
        
        transform.rotation = Quaternion.Euler(0, 0, z);
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
}
