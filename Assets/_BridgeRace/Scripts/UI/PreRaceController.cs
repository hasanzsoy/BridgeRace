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


    // =====================================================
    // CHARACTER REFERENCES
    // =====================================================

    private PlayerController player;

    private CharacterStack playerStack;

    private PlayerMagnetPowerUp playerMagnetPowerUp;

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


        // PreRace ekranı açık.
        if (preRacePanel != null)
        {
            preRacePanel.SetActive(
                true
            );
        }


        // Yarış başlamadan joystick yok.
        if (joystickTouchArea != null)
        {
            joystickTouchArea.SetActive(
                false
            );
        }


        // Yarış başlamadan Pause yok.
        if (pauseButton != null)
        {
            pauseButton.SetActive(
                false
            );
        }


        // =============================================
        // PLAYER BEKLESİN
        // =============================================

        if (player != null)
        {
            player.enabled =
                false;
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
        // UPGRADE'LARI HAZIRLA
        // =================================================

        int startingBrickAmount =
            0;


        bool magnetReady =
            false;


        if (upgradeController != null)
        {
            // =============================================
            // STARTING BRICKS
            // =============================================

            startingBrickAmount =
                upgradeController
                .ConsumeStartingBricks();


            // =============================================
            // MAGNET
            // =============================================

            magnetReady =
                upgradeController
                .ConsumeMagnet();
        }


        // =================================================
        // STARTING BRICKLERİ VER
        // =================================================

        if (playerStack != null &&
            startingBrickAmount > 0)
        {
            playerStack.AddBonusBricks(
                startingBrickAmount
            );
        }


        // =================================================
        // PRE-RACE UI KAPAT
        // =================================================

        if (preRacePanel != null)
        {
            preRacePanel.SetActive(
                false
            );
        }


        // =================================================
        // PLAYER AÇ
        // =================================================

        if (player != null)
        {
            player.enabled =
                true;
        }


        // =================================================
        // AI'LARI AÇ
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
        // RACE START EVENT
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
                    "Magnet satın alındı fakat Player üzerinde " +
                    "PlayerMagnetPowerUp bulunamadı!"
                );
            }
        }
    }
}