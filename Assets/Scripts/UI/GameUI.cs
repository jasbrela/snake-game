using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;

        private void Awake()
        {
            if (gameOverPanel.activeInHierarchy) gameOverPanel.SetActive(false);
            GameManager.Instance.SendOnGameOverStatusChangedCallback(OnGameOverStatusChanged);
        }

        /// <summary>
        /// Show or hide the Game Over panel.
        /// </summary>
        private void OnGameOverStatusChanged()
        {
            gameOverPanel.SetActive(GameManager.Instance.IsGameOver());
        }
    
        /// <summary>
        /// Loads the Main Menu scene.
        /// </summary>
        public void OnPressMainMenu()
        {
            SceneManager.LoadScene(Scenes.MainMenu.ToString());
        }
    }
}
