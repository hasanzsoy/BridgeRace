using UnityEngine;

public class PlayerSpeedPowerUp : MonoBehaviour
{
    // =====================================================
    // SPEED SETTINGS
    // =====================================================

    [Header("Speed Settings")]

    [SerializeField]
    private float speedMultiplier = 1.5f;


    // =====================================================
    // OPTIONAL VISUAL
    // =====================================================

    [Header("Optional Visual")]

    [SerializeField]
    private GameObject speedVisual;


    // =====================================================
    // REFERENCES
    // =====================================================

    private PlayerController player;


    // =====================================================
    // RUNTIME
    // =====================================================

    private bool speedActive;

    private float originalMoveSpeed;

    private float speedEndTime;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        player =
            GetComponent<PlayerController>();


        if (player == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde PlayerController bulunamadı!"
            );
        }


        if (speedVisual != null)
        {
            speedVisual.SetActive(
                false
            );
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!speedActive)
        {
            return;
        }


        // Pause sırasında Time.time ilerlemediği için
        // Speed süresi de otomatik olarak durur.
        if (Time.time <
            speedEndTime)
        {
            return;
        }


        StopSpeedBoost();
    }


    // =====================================================
    // ACTIVATE
    // =====================================================

    public void ActivateSpeedBoost(
        float duration)
    {
        if (duration <= 0f)
        {
            return;
        }


        if (player == null)
        {
            return;
        }


        // Zaten aktifse tekrar başlangıç hızını
        // yanlış kaydetmeyelim.
        if (speedActive)
        {
            return;
        }


        speedActive =
            true;


        // =============================================
        // NORMAL HIZI KAYDET
        // =============================================

        originalMoveSpeed =
            player.CurrentMoveSpeed;


        // =============================================
        // BOOST HIZINI HESAPLA
        //
        // Örnek:
        //
        // Normal = 5
        // Multiplier = 1.5
        //
        // Boost = 7.5
        // =============================================

        float boostedSpeed =
            originalMoveSpeed *
            speedMultiplier;


        player.SetMoveSpeed(
            boostedSpeed
        );


        speedEndTime =
            Time.time +
            duration;


        if (speedVisual != null)
        {
            speedVisual.SetActive(
                true
            );
        }


        Debug.Log(
            "SPEED BOOST AKTİF! " +
            "Normal: " +
            originalMoveSpeed +
            " | Boost: " +
            boostedSpeed +
            " | Süre: " +
            duration
        );
    }


    // =====================================================
    // STOP
    // =====================================================

    private void StopSpeedBoost()
    {
        if (!speedActive)
        {
            return;
        }


        speedActive =
            false;


        if (player != null)
        {
            player.SetMoveSpeed(
                originalMoveSpeed
            );
        }


        if (speedVisual != null)
        {
            speedVisual.SetActive(
                false
            );
        }


        Debug.Log(
            "SPEED BOOST BİTTİ! " +
            "Hız tekrar: " +
            originalMoveSpeed
        );
    }


    // =====================================================
    // SAFETY
    // =====================================================

    private void OnDestroy()
    {
        if (speedActive &&
            player != null)
        {
            player.SetMoveSpeed(
                originalMoveSpeed
            );
        }
    }
}