using System.Collections;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI tickText;
        [SerializeField] private GameObject tickTextGameObject;

        private void Awake()
        {
            if (gameOverPanel.activeInHierarchy) gameOverPanel.SetActive(false);
            GameManager.Instance.SendOnGameOverStatusChangedCallback(OnGameOverStatusChanged);
            GameManager.Instance.SendOnTickCallback(OnTick);
        }

        /// <summary>
        /// Show or hide the Game Over panel.
        /// </summary>
        private void OnGameOverStatusChanged()
        {
            // TODO: Show Game Over Panel when game starts
            gameOverPanel.SetActive(GameManager.Instance.IsGameOver());
        }
    
        /// <summary>
        /// Loads the Main Menu scene.
        /// </summary>
        public void OnPressMainMenu()
        {
            SceneManager.LoadScene(Scenes.MainMenu.ToString());
        }

        /// <summary>
        /// Show the timer.
        /// </summary>
        /// <param name="tickCounter">The current second of the timer</param>
        private void OnTick(int tickCounter)
        {
            switch (tickCounter)
            {
                case 3:
                    tickTextGameObject.SetActive(true);
                    break;
                case -1:
                    tickTextGameObject.SetActive(false);
                    return;
            }

            tickText.text = tickCounter.ToString();
        }
    }
}
