using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public delegate void OnGameStarts();
    private OnGameStarts _onGameStarts;
    
    public delegate void OnPressRetry();
    private OnPressRetry _onPressRetry;
    
    public delegate void OnGameOver();
    private OnGameOver _onGameOver;
    
    public delegate void OnVictory(string name, Color color);
    private OnVictory _onVictory;
    
    public delegate Vector3 GetSpawnPoint();
    private GetSpawnPoint _getSpawnPoint;
    
    public delegate Vector3 GetDeadPoint();
    private GetDeadPoint _getDeadPoint;
    
    public delegate void OnClickMainMenu();
    private OnClickMainMenu _onClickMainMenu;
    
    public delegate void OnTick(int tick);
    private OnTick _onTick;
    #endregion
    
    #region Variables
    private int _tickCount = 3;
    private int _tickDefaultValue;
    private int _maxSpawnPointIndex;
    private static bool _isMultiplayer;
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
        if (SceneManager.GetActiveScene().name != Scenes.SingleplayerGame.ToString()) return;
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
            _onGameStartsForTheFirstTime?.Invoke();
        }
        _onGameStarts?.Invoke();

        _gameOver = false;
    }

    #region Receive Callbacks

    /// <summary>
    /// Used to send callbacks to be called when the game starts for the first time.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnGameStartsForTheFirstTimeCallback(OnGameStartsForTheFirstTime method)
    {
        _onGameStartsForTheFirstTime += method;
    }
    
    /// <summary>
    /// Used to send callbacks to be called when the game starts, whether it's first time or not.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnGameStartsCallback(OnGameStarts method)
    {
        _onGameStarts += method;
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
    /// Used to send callbacks to be called when Player presses the Main Menu button.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnPressMainMenu(OnClickMainMenu method)
    {
        _onClickMainMenu += method;
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
    /// Used to send callbacks to be called when a human player wins.
    /// </summary>
    /// <param name="method">A void method to be called</param>
    public void SendOnVictoryCallback(OnVictory method)
    {
        _onVictory += method;
    }

    /// <summary>
    /// Used to send callbacks to be called when Player asks for a certain point to teleport..
    /// </summary>
    /// <param name="spawnMethod">A Vector3 method to be called on Get Spawn Point</param>
    /// <param name="deadMethod">A Vector3 method to be called on Get Dead Point</param>

    public void SendPointsCallback(GetSpawnPoint spawnMethod, GetDeadPoint deadMethod)
    {
        _getSpawnPoint += spawnMethod;
        _getDeadPoint += deadMethod;
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
    /// Calls the onVictory delegate.
    /// </summary>
    /// <param name="nick">The snake's nickname</param>
    /// <param name="color">The snake's color</param>
    public void OnHumanPlayerWins(string nick, Color color)
    {
        if (!_isMultiplayer) return;
        _onVictory?.Invoke(nick, color);
    }

    /// <summary>
    /// Resets the game then restarts it.
    /// </summary>
    public void Retry()
    {
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
    
    /// <summary>
    /// Called by the Player to get the dead position.
    /// </summary>
    /// <returns>A Vector3 of the dead position.</returns>
    public Vector3 GetDeadPosition()
    {
        return _getDeadPoint();
    }

    /// <summary>
    /// Change isMultiplayer status.
    /// </summary>
    /// <param name="isMultiplayer">Is this game multiplayer?</param>
    public static void SetMultiplayerGame(bool isMultiplayer)
    {
        _isMultiplayer = isMultiplayer;
    }

    /// <returns>Is this a multiplayer game?</returns>
    public static bool IsAMultiplayerGame()
    {
        return _isMultiplayer;
    }

    /// <summary>
    /// Reset the game
    /// </summary>
    public void ClickMainMenu()
    {
        _onClickMainMenu?.Invoke();
    }
}