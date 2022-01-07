using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnPressStart()
    {
        SceneManager.LoadScene("Game");
    }
}
