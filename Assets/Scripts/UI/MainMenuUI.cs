using System;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject quitButton;
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                quitButton.SetActive(false);
            }
        }

        /// <summary>
        /// Loads the Singleplayer scene.
        /// </summary>
        public void OnPressSingleplayer()
        {
            SceneManager.LoadScene(Scenes.SingleplayerGame.ToString());
        }
        
        /// <summary>
        /// Loads the Multiplayer scene.
        /// </summary>
        public void OnPressMultiplayer()
        {
            SceneManager.LoadScene(Scenes.Preparation.ToString());
        }
        
        /// <summary>
        /// Closes the game
        /// </summary>
        public void OnPressQuit()
        {
            Application.Quit();
        }
    }
}
