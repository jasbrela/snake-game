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

    public void EndGame()
    {
        // TODO: hide snakes
        _gameOver = true;
    }

    public void ResetGame()
    {
        // TODO: reset and show snakes
        _gameOver = false;
    }
    
    public bool IsGameOver()
    {
        return _gameOver;
    }
}