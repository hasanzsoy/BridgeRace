using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectionManager : MonoBehaviour
{
    [Header("Gameplay Scene")]
    [SerializeField]
    private string gameplaySceneName =
        "02_Level_01";


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


    private void StartGame()
    {
        SceneManager.LoadScene(
            gameplaySceneName
        );
    }
}