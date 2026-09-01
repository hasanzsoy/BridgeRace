using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryNavigationController : MonoBehaviour
{
    public void GoToDifficulty()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("DifficultySelect");
    }

    public void GoToHome()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("01_MainMenu");
    }
}