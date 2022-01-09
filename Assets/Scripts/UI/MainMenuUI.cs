using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        /// <summary>
        /// Loads the Game scene.
        /// </summary>
        public void OnPressStart()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
