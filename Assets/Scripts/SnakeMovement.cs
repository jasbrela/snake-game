using System;
using System.Collections.Generic;
using UnityEngine;

public enum Directions {
    Left, Right, Up, Down
}
public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject bodyPart;
    [SerializeField] private LayerMask obstacle;
    [SerializeField] private Transform point;
    [SerializeField] private int initialSize;
    
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
    private float _actualSpeed;

    void Start()
    {
        ResetSnake();
        Controls.Enable();
        Invoke(nameof(Move), 0);
    }

    private void ResetSnake()
    {
        transform.rotation = Quaternion.identity;
        _actualSpeed = speed * 0.1f;
        SetUpControls();
        _bodyParts.Clear();
        _bodyParts.Add(transform);
        SetUpInitialSize();
    }

    private void SetUpInitialSize()
    {
        for (var i = 0; i < initialSize; i++)
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
        if (point.GetComponent<BoxCollider2D>().IsTouchingLayers(obstacle))
        {
            GameOver();
            return;
        }

        for (int i = _bodyParts.Count - 1; i > 0; i--)
        {
            _bodyParts[i].rotation = _bodyParts[i - 1].rotation;
            _bodyParts[i].position = _bodyParts[i - 1].position;
        }

        Transform t = transform;
        t.position += t.right;
        
        Invoke(nameof(Move), _actualSpeed);
        _canMove = true;
    }

    private void GameOver()
    {
        Controls.Disable();
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

            IncreaseSize();
            AddPowerUp(block.Type);
            block.RespawnBlock();
        }
    }

    private void AddPowerUp(BlockType type)
    {
        // todo: create separate script for power-ups
    }
    
    private void IncreaseSize()
    {
        Transform reference = _bodyParts[_bodyParts.Count - 1];
        
        GameObject block = Instantiate(bodyPart, reference.position - reference.right, reference.rotation);
        _bodyParts.Add(block.transform);
        block.transform.parent = transform.parent;
    }
}
