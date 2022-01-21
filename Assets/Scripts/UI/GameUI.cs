using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private GameObject victoryMessage;
        [SerializeField] private GameObject defeatMessage;
        [SerializeField] private TextMeshProUGUI tickText;
        [SerializeField] private GameObject tickTextGameObject;

        private bool humanWin;

        private void Awake()
        {
            if (gameOverPanel.activeInHierarchy) gameOverPanel.SetActive(false);
            GameManager.Instance.SendOnPressRetryCallback(HideGameOverPanel);
            GameManager.Instance.SendOnGameOverCallback(ShowGameOverPanel);
            GameManager.Instance.SendOnTickCallback(OnTick);
            GameManager.Instance.SendOnVictoryCallback(ShowPlayerWonMessage);
        }

        /// <summary>
        /// Hide the Game Over panel.
        /// </summary>
        private void HideGameOverPanel()
        {
            gameOverPanel.SetActive(false);
        }

        /// <summary>
        /// Shows a message telling who won.
        /// </summary>
        /// <param name="name">Player's name</param>
        /// <param name="color">Player's color</param>
        private void ShowPlayerWonMessage(string name, Color color)
        {
            humanWin = true;
            playerName.text = name;
            playerName.color = color;
        }

        /// <summary>
        /// If the a player won, show the victory message, otherwise show defeat message.
        /// </summary>
        private void ShowMessage()
        {
            if (!humanWin)
            {
                victoryMessage.SetActive(false);
                defeatMessage.SetActive(true);
            }
            else
            {
                victoryMessage.SetActive(true);
                defeatMessage.SetActive(false);
            }
        }
        
        /// <summary>
        /// Show the Game Over panel.
        /// </summary>
        private void ShowGameOverPanel()
        {
            ShowMessage();
            gameOverPanel.SetActive(true);
        }
    
        /// <summary>
        /// Loads the Main Menu scene.
        /// </summary>
        public void OnPressMainMenu()
        {
            GameManager.Instance.ClickMainMenu();
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

        /// <summary>
        /// Restarts the game.
        /// </summary>
        public void OnPressRetry()
        {
            humanWin = false;
            GameManager.Instance.Retry();
        }
    }
}
