using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public void OnPressStart()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
