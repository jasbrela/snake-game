using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
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
    }
}
