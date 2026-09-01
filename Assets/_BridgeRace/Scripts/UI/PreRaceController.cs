using UnityEngine;

public class PreRaceController : MonoBehaviour
{
    // =====================================================
    // UI REFERENCES
    // =====================================================

    [Header("UI References")]

    [SerializeField]
    private GameObject preRacePanel;

    [SerializeField]
    private GameObject joystickTouchArea;

    [SerializeField]
    private GameObject pauseButton;


    // =====================================================
    // UPGRADE REFERENCES
    // =====================================================

    [Header("Upgrade References")]

    [SerializeField]
    private PreRaceUpgradeController upgradeController;


    // =====================================================
    // POWER-UP SETTINGS
    // =====================================================

    [Header("Power-Up Settings")]

    [SerializeField]
    private float magnetDuration = 5f;

    [SerializeField]
    private float speedDuration = 3f;


    // =====================================================
    // CHARACTER REFERENCES
    // =====================================================

    private PlayerController player;

    private CharacterStack playerStack;

    private PlayerMagnetPowerUp playerMagnetPowerUp;

    private PlayerSpeedPowerUp playerSpeedPowerUp;

    private AIController[] aiControllers;


    // =====================================================
    // RUNTIME
    // =====================================================

    private bool raceStarted;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        FindCharacters();

        PreparePreRace();
    }


    // =====================================================
    // FIND CHARACTERS
    // =====================================================

    private void FindCharacters()
    {
        player =
            FindFirstObjectByType<PlayerController>();


        if (player != null)
        {
            playerStack =
                player.GetComponent<CharacterStack>();


            playerMagnetPowerUp =
                player.GetComponent<PlayerMagnetPowerUp>();


            playerSpeedPowerUp =
                player.GetComponent<PlayerSpeedPowerUp>();
        }


        aiControllers =
            FindObjectsByType<AIController>(
                FindObjectsSortMode.None
            );
    }


    // =====================================================
    // PRE-RACE
    // =====================================================

    private void PreparePreRace()
    {
        raceStarted =
            false;


        if (preRacePanel != null)
        {
            preRacePanel.SetActive(
                true
            );
        }


        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(
                false
            );
        }


        if (pauseButton != null)
        {
            pauseButton.SetActive(
                false
            );
        }


        if (player != null)
        {
            player.enabled =
                false;
        }


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


                aiControllers[i].enabled =
                    false;
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


        raceStarted =
            true;


        // =================================================
        // UPGRADE DURUMLARI
        // =================================================

        int startingBrickAmount =
            0;


        bool magnetReady =
            false;


        bool speedReady =
            false;


        if (upgradeController != null)
        {
            startingBrickAmount =
                upgradeController
                .ConsumeStartingBricks();


            magnetReady =
                upgradeController
                .ConsumeMagnet();


            speedReady =
                upgradeController
                .ConsumeSpeed();
        }


        // =================================================
        // STARTING BRICKS
        // =================================================

        if (playerStack != null &&
            startingBrickAmount > 0)
        {
            playerStack.AddBonusBricks(
                startingBrickAmount
            );
        }


        // =================================================
        // PRE-RACE UI
        // =================================================

        if (preRacePanel != null)
        {
            preRacePanel.SetActive(
                false
            );
        }


        // =================================================
        // PLAYER
        // =================================================

        if (player != null)
        {
            player.enabled =
                true;
        }


        // =================================================
        // AI
        // =================================================

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


                aiControllers[i].enabled =
                    true;
            }
        }


        // =================================================
        // JOYSTICK
        // =================================================

        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(
                true
            );
        }


        // =================================================
        // PAUSE
        // =================================================

        if (pauseButton != null)
        {
            pauseButton.SetActive(
                true
            );
        }


        // =================================================
        // RACE EVENT
        // =================================================

        EventManager.RaceStarted();


        // =================================================
        // MAGNET
        // =================================================

        if (magnetReady)
        {
            if (playerMagnetPowerUp != null)
            {
                playerMagnetPowerUp
                    .ActivateMagnet(
                        magnetDuration
                    );
            }
            else
            {
                Debug.LogWarning(
                    "Magnet satın alındı fakat " +
                    "PlayerMagnetPowerUp bulunamadı!"
                );
            }
        }


        if (speedReady)
        {
            if (playerSpeedPowerUp != null)
            {
                playerSpeedPowerUp
                    .ActivateSpeedBoost(
                        speedDuration
                    );
            }
            else
            {
                Debug.LogWarning(
                    "Speed satın alındı fakat " +
                    "PlayerSpeedPowerUp bulunamadı!"
                );
            }
        }
    }
}