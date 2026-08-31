using UnityEngine;

public class PreRaceController : MonoBehaviour
{
    [Header("UI References")]

    [SerializeField]
    private GameObject preRacePanel;

    [SerializeField]
    private GameObject joystickTouchArea;

    [SerializeField]
    private GameObject pauseButton;

    private PlayerController player;

    private AIController[] aiControllers;

    private bool raceStarted;

    private void Awake()
    {
        FindCharacters();

        PreparePreRace();
    }

    private void FindCharacters()
    {
        player =
            FindFirstObjectByType<PlayerController>();


        aiControllers =
            FindObjectsByType<AIController>(
                FindObjectsSortMode.None
            );
    }
    private void PreparePreRace()
    {
        raceStarted = false;


        // Upgrade ekranını aç.
        if (preRacePanel != null)
        {
            preRacePanel.SetActive(true);
        }


        // Yarış başlamadığı için joystick kapalı.
        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(false);
        }


        // Yarış başlamadığı için Pause butonu kapalı.
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }


        // =============================================
        // PLAYER BEKLESİN
        // =============================================

        if (player != null)
        {
            player.enabled = false;
        }


        // =============================================
        // AI'LAR BEKLESİN
        // =============================================

        if (aiControllers != null)
        {
            for (int i = 0;
                 i < aiControllers.Length;
                 i++)
            {
                if (aiControllers[i] == null)
                {
                    continue;
                }


                aiControllers[i].enabled = false;
            }
        }
    }


    // =====================================================
    // START RACE
    // =====================================================

    public void StartRace()
    {
        if (raceStarted)
        {
            return;
        }


        raceStarted = true;


        // Upgrade ekranını kapat.
        if (preRacePanel != null)
        {
            preRacePanel.SetActive(false);
        }


        // =============================================
        // PLAYER
        // =============================================

        if (player != null)
        {
            player.enabled = true;
        }


        // =============================================
        // AI
        // =============================================

        if (aiControllers != null)
        {
            for (int i = 0;
                 i < aiControllers.Length;
                 i++)
            {
                if (aiControllers[i] == null)
                {
                    continue;
                }


                aiControllers[i].enabled = true;
            }
        }


        // =============================================
        // JOYSTICK
        // =============================================

        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(true);
        }


        // =============================================
        // PAUSE
        // =============================================

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }


        // =============================================
        // RACE START EVENT
        // =============================================

        EventManager.RaceStarted();
    }
}