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
    public delegate void OnPressRetry();
    private OnPressRetry _onPressRetry;
    
    public delegate void OnGameOverStatusChanged();
    private OnGameOverStatusChanged _onGameOverStatusChanged;
    
    public delegate Transform GetSpawnPoint();
    private GetSpawnPoint _getSpawnPoint;
    #endregion

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

    #region Receive Callbacks

    /// <summary>
    /// Used to send callbacks to be called when Player presses the Retry button.
    /// </summary>
    /// <param name="method">A method to be called</param>
    public void SendOnPressRetryCallback(OnPressRetry method)
    {
        _onPressRetry += method;
    }
        
    /// <summary>
    /// Used to send callbacks to be called when Player loses or restart game.
    /// </summary>
    /// <param name="method">A method to be called</param>
    public void SendOnGameOverStatusChangedCallback(OnGameOverStatusChanged method)
    {
        _onGameOverStatusChanged += method;
    }

    /// <summary>
    /// Used to send callbacks to be called when Player asks for a spawn point..
    /// </summary>
    /// <param name="method">A method to be called</param>
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
        ChangeGameOverStatus(true);
    }

    /// <summary>
    /// Reset the game and restart the game.
    /// </summary>
    public void ResetGame()
    {
        _onPressRetry?.Invoke();
        ChangeGameOverStatus(false);
    }
    
    /// <summary>
    /// Used to check if the game is or not over.
    /// </summary>
    /// <returns>A bool regarding the game status</returns>
    public bool IsGameOver()
    {
        return _gameOver;
    }

    /// <summary>
    /// Changes the game over to a desired status
    /// </summary>
    /// <param name="isOver">Desired status</param>
    private void ChangeGameOverStatus(bool isOver)
    {
        _gameOver = isOver;
        _onGameOverStatusChanged();
    }

    /// <summary>
    /// Called by the Player to get the next spawn point.
    /// </summary>
    /// <returns>A transform of the next spawn point.</returns>
    public Transform GetNextSpawnPoint()
    {
        return _getSpawnPoint();
    }
}