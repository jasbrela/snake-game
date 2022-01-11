using System.Collections;
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
    #region Callbacks
    public delegate void OnGameStartsForTheFirstTime();
    private OnGameStartsForTheFirstTime _onGameStartsForTheFirstTime;
    
    public delegate void OnPressRetry();
    private OnPressRetry _onPressRetry;
    
    public delegate void OnGameOver();
    private OnGameOver _onGameOver;
    
    public delegate Vector3 GetSpawnPoint();
    private GetSpawnPoint _getSpawnPoint;
    
    public delegate void OnTick(int tick);
    private OnTick _onTick;
    #endregion
    #region Variables
    private int _tickCount = 3;
    private int _tickDefaultValue;
    private int _maxSpawnPointIndex;
    private bool _gameOver = true;
    private bool _hasStarted;
    #endregion

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
        
        _tickDefaultValue = _tickCount;
    }

    private void Start()
    {
        Retry();
    }
    
    /// <summary>
    /// Waits for the timer to end, then start the game.
    /// </summary>
    private IEnumerator StartGame()
    {
        yield return new WaitForEndOfFrame();
        _onPressRetry?.Invoke();
        
        _onTick(_tickCount);
        while (_tickCount >= 0)
        {
            yield return new WaitForSeconds(1);
            _tickCount--;
            _onTick(_tickCount);
        }

        if (!_hasStarted)
        {
            _hasStarted = true;
            _onGameStartsForTheFirstTime();
        }

        _gameOver = false;
    }

    #region Receive Callbacks
    /// <summary>
    /// Used to send callbacks to be called when the game starts.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnGameStartsForTheFirstTimeCallback(OnGameStartsForTheFirstTime method)
    {
        _onGameStartsForTheFirstTime += method;
    }
    
    /// <summary>
    /// Used to send callbacks to be called when the timer ticks.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnTickCallback(OnTick method)
    {
        _onTick += method;
    }
    
    /// <summary>
    /// Used to send callbacks to be called when Player presses the Retry button.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnPressRetryCallback(OnPressRetry method)
    {
        _onPressRetry += method;
    }
        
    /// <summary>
    /// Used to send callbacks to be called when the game is over.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnGameOverCallback(OnGameOver method)
    {
        _onGameOver += method;
    }

    /// <summary>
    /// Used to send callbacks to be called when Player asks for a spawn point..
    /// </summary>
    /// <param name="method">A Vector3 method to be called</param>
    public void SendGetSpawnPointsCallback(GetSpawnPoint method)
    {
        _getSpawnPoint += method;
    }
    #endregion

    /// <summary>
    /// Change the game status to over.
    /// </summary>
    public void EndGame()
    {
        _gameOver = true;
        _onGameOver?.Invoke();
    }

    /// <summary>
    /// Resets the game then restarts it.
    /// </summary>
    public void Retry()
    {
        // BUG: Snake is getting faster and faster on press retry.
        _tickCount = _tickDefaultValue;
        StartCoroutine(StartGame());
    }

    /// <summary>
    /// Used to check if the game is or not over.
    /// </summary>
    /// <returns>A bool regarding the game over status</returns>
    public bool IsGameOver()
    {
        return _gameOver;
    }

    /// <summary>
    /// Called by the Player to get the next spawn point position.
    /// </summary>
    /// <returns>A Vector3 of the next spawn point position.</returns>
    public Vector3 GetNextSpawnPointPosition()
    {
        return _getSpawnPoint();
    }
}