using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject settingsPanel;

    [Header("Gameplay UI References")]

    [SerializeField]
    private GameObject joystickTouchArea;

    [Header("Scene Settings")]

    [SerializeField]
    private string mainMenuSceneName = "01_MainMenu";

    private bool isPaused;
    private void Start()
    {
        Time.timeScale = 1f;

        isPaused = false;


        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(true);
        }
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;

        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(false);
        }


        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!isPaused)
        {
            return;
        }

        Time.timeScale = 1f;

        isPaused = false;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("Pause Menu için SettingsPanel atanmadı.");

            return;
        }

        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        isPaused = false;

        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(true);
        }


        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}