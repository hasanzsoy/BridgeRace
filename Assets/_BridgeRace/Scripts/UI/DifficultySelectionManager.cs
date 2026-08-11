using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectionManager :
    MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField]
    private string gameplaySceneName =
        "02_Level_01";

    [SerializeField]
    private string mainMenuSceneName =
        "01_MainMenu";


    public void SelectEasy()
    {
        GameSettings.SelectedDifficulty =
            AIDifficulty.Easy;

        StartGame();
    }


    public void SelectNormal()
    {
        GameSettings.SelectedDifficulty =
            AIDifficulty.Normal;

        StartGame();
    }


    public void SelectHard()
    {
        GameSettings.SelectedDifficulty =
            AIDifficulty.Hard;

        StartGame();
    }


    public void BackToMainMenu()
    {
        SceneManager.LoadScene(
            mainMenuSceneName
        );
    }


    private void StartGame()
    {
        SceneManager.LoadScene(
            gameplaySceneName
        );
    }
}