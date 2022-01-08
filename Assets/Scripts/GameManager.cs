using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("Game Manager");
                go.AddComponent<GameManager>();
            }
            return _instance;
        }
    }
    #endregion
    
    public delegate void OnPressRetry();
    private OnPressRetry _onPressRetry;
    
    public delegate void OnGameOverStatusChanged();
    private OnGameOverStatusChanged _onGameOverStatusChanged;

    private int _lastPickedSpawnPoint;
    private int _maxSpawnPointIndex;
    private bool _gameOver;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        ResetGame();
    }

    public void SendOnPressRetryCallback(OnPressRetry callback)
    {
        _onPressRetry += callback;
    }
    
    public void SendOnGameOverStatusChangedCallback(OnGameOverStatusChanged callback)
    {
        _onGameOverStatusChanged += callback;
    }

    public void EndGame()
    {
        ChangeGameOverStatus(true);
    }

    public void ResetGame()
    {
        ChangeGameOverStatus(false);
        _onPressRetry?.Invoke();
    }
    
    public bool IsGameOver()
    {
        return _gameOver;
    }

    private void ChangeGameOverStatus(bool isOver)
    {
        _gameOver = isOver;
        _onGameOverStatusChanged();
    }

    public void SetMaxSpawnPoints(int index)
    {
        _maxSpawnPointIndex = index;
    }

    public int GetNextSpawnPoint()
    {
        if (_maxSpawnPointIndex == _lastPickedSpawnPoint)
        {
            _lastPickedSpawnPoint = 0;
        }
        else
        {
            _lastPickedSpawnPoint++;
        }
        
        return _lastPickedSpawnPoint;
    }
}