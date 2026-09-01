using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreRaceUpgradeController : MonoBehaviour
{
    // =====================================================
    // STARTING BRICK SETTINGS
    // =====================================================

    [Header("Starting Brick Settings")]

    [SerializeField]
    private int brickPrice = 200;

    [SerializeField]
    private int maximumStartingBricks = 5;


    // =====================================================
    // MAGNET SETTINGS
    // =====================================================

    [Header("Magnet Settings")]

    [SerializeField]
    private int magnetPrice = 300;


    // =====================================================
    // STARTING BRICK UI
    // =====================================================

    [Header("Starting Brick UI")]

    [SerializeField]
    private TMP_Text startingBrickAmountText;

    [SerializeField]
    private Button brickBuyButton;


    // =====================================================
    // MAGNET UI
    // =====================================================

    [Header("Magnet UI")]

    [SerializeField]
    private TMP_Text magnetStateText;

    [SerializeField]
    private Button magnetBuyButton;


    // =====================================================
    // GENERAL UI
    // =====================================================

    [Header("General UI")]

    [SerializeField]
    private TMP_Text feedbackText;


    // =====================================================
    // RUNTIME
    // =====================================================

    private int purchasedStartingBricks;

    private bool magnetPurchased;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // Her yarış başladığında upgrade seçimleri
        // yeniden sıfırdan başlar.
        //
        // Gold kalıcıdır ama satın alınan yarışlık
        // avantajlar kalıcı değildir.

        purchasedStartingBricks = 0;

        magnetPurchased = false;


        RefreshAllUI();

        ClearFeedback();
    }


    // =====================================================
    // BUY STARTING BRICK
    // =====================================================

    public void BuyStartingBrick()
    {
        // Max 5 kontrolü.
        if (purchasedStartingBricks >=
            maximumStartingBricks)
        {
            ShowFeedback(
                "MAX BRICKS!"
            );

            return;
        }


        int currentGold =
            SaveManager.LoadGold();


        // Gold yeterli mi?
        if (currentGold <
            brickPrice)
        {
            ShowFeedback(
                "NOT ENOUGH GOLD!"
            );

            return;
        }


        // Gold düş.
        int newGold =
            currentGold -
            brickPrice;


        SaveManager.SaveGold(
            newGold
        );


        // Gold UI event üzerinden güncellensin.
        EventManager.GoldChanged(
            newGold
        );


        purchasedStartingBricks++;


        RefreshStartingBrickUI();


        ShowFeedback(
            "+1 BRICK READY!"
        );
    }


    // =====================================================
    // BUY MAGNET
    // =====================================================

    public void BuyMagnet()
    {
        // Magnet bu yarış için zaten alındıysa
        // ikinci kere satın alınamaz.

        if (magnetPurchased)
        {
            ShowFeedback(
                "MAGNET ALREADY READY!"
            );

            return;
        }


        int currentGold =
            SaveManager.LoadGold();


        // Gold yeterli değil.
        if (currentGold <
            magnetPrice)
        {
            ShowFeedback(
                "NOT ENOUGH GOLD!"
            );

            return;
        }


        // =================================================
        // GOLD DÜŞÜR
        // =================================================

        int newGold =
            currentGold -
            magnetPrice;


        SaveManager.SaveGold(
            newGold
        );


        EventManager.GoldChanged(
            newGold
        );


        // =================================================
        // MAGNET HAZIR
        // =================================================

        magnetPurchased = true;


        RefreshMagnetUI();


        ShowFeedback(
            "MAGNET READY!"
        );
    }


    // =====================================================
    // STARTING BRICK GET / CONSUME
    // =====================================================

    public int GetPurchasedStartingBricks()
    {
        return purchasedStartingBricks;
    }


    public int ConsumeStartingBricks()
    {
        int amount =
            purchasedStartingBricks;


        purchasedStartingBricks =
            0;


        RefreshStartingBrickUI();


        return amount;
    }


    // =====================================================
    // MAGNET GET / CONSUME
    // =====================================================

    public bool IsMagnetPurchased()
    {
        return magnetPurchased;
    }


    public bool ConsumeMagnet()
    {
        // Magnet alınmadıysa false dön.
        if (!magnetPurchased)
        {
            return false;
        }


        // Bu yarış için satın alınan Magnet'i tüket.
        magnetPurchased = false;


        return true;
    }


    // =====================================================
    // REFRESH ALL UI
    // =====================================================

    private void RefreshAllUI()
    {
        RefreshStartingBrickUI();

        RefreshMagnetUI();
    }


    // =====================================================
    // STARTING BRICK UI
    // =====================================================

    private void RefreshStartingBrickUI()
    {
        if (startingBrickAmountText != null)
        {
            startingBrickAmountText.text =
                purchasedStartingBricks +
                " / " +
                maximumStartingBricks;
        }


        if (brickBuyButton != null)
        {
            brickBuyButton.interactable =
                purchasedStartingBricks <
                maximumStartingBricks;
        }
    }


    // =====================================================
    // MAGNET UI
    // =====================================================

    private void RefreshMagnetUI()
    {
        if (magnetStateText != null)
        {
            magnetStateText.text =
                magnetPurchased
                    ? "READY!"
                    : "READY: NO";
        }


        if (magnetBuyButton != null)
        {
            // Satın alınca buton artık basılamaz.
            magnetBuyButton.interactable =
                !magnetPurchased;
        }
    }


    // =====================================================
    // FEEDBACK
    // =====================================================

    private void ShowFeedback(
        string message)
    {
        if (feedbackText == null)
        {
            return;
        }


        feedbackText.text =
            message;
    }


    private void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text =
                "";
        }
    }
}