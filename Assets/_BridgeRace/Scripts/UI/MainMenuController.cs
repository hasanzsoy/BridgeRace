using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("DifficultySelect");
    }

    public void ExitGame()
    {
        Debug.Log("Oyundan çıkış yapılıyor...");

        Application.Quit();
    }
}