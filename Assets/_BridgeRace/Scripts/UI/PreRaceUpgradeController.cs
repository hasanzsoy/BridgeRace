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
    // SPEED SETTINGS
    // =====================================================

    [Header("Speed Settings")]

    [SerializeField]
    private int speedPrice = 250;


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
    // SPEED UI
    // =====================================================

    [Header("Speed UI")]

    [SerializeField]
    private TMP_Text speedStateText;

    [SerializeField]
    private Button speedBuyButton;


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

    private bool speedPurchased;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        purchasedStartingBricks =
            0;


        magnetPurchased =
            false;


        speedPurchased =
            false;


        RefreshAllUI();

        ClearFeedback();
    }


    // =====================================================
    // BUY STARTING BRICK
    // =====================================================

    public void BuyStartingBrick()
    {
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


        if (currentGold <
            brickPrice)
        {
            ShowFeedback(
                "NOT ENOUGH GOLD!"
            );

            return;
        }


        int newGold =
            currentGold -
            brickPrice;


        SaveManager.SaveGold(
            newGold
        );


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
        if (magnetPurchased)
        {
            ShowFeedback(
                "MAGNET ALREADY READY!"
            );

            return;
        }


        int currentGold =
            SaveManager.LoadGold();


        if (currentGold <
            magnetPrice)
        {
            ShowFeedback(
                "NOT ENOUGH GOLD!"
            );

            return;
        }


        int newGold =
            currentGold -
            magnetPrice;


        SaveManager.SaveGold(
            newGold
        );


        EventManager.GoldChanged(
            newGold
        );


        magnetPurchased =
            true;


        RefreshMagnetUI();


        ShowFeedback(
            "MAGNET READY!"
        );
    }


    // =====================================================
    // BUY SPEED
    // =====================================================

    public void BuySpeed()
    {
        if (speedPurchased)
        {
            ShowFeedback(
                "SPEED ALREADY READY!"
            );

            return;
        }


        int currentGold =
            SaveManager.LoadGold();


        if (currentGold <
            speedPrice)
        {
            ShowFeedback(
                "NOT ENOUGH GOLD!"
            );

            return;
        }


        int newGold =
            currentGold -
            speedPrice;


        SaveManager.SaveGold(
            newGold
        );


        EventManager.GoldChanged(
            newGold
        );


        speedPurchased =
            true;


        RefreshSpeedUI();


        ShowFeedback(
            "SPEED READY!"
        );
    }


    // =====================================================
    // STARTING BRICK
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
    // MAGNET
    // =====================================================

    public bool IsMagnetPurchased()
    {
        return magnetPurchased;
    }


    public bool ConsumeMagnet()
    {
        if (!magnetPurchased)
        {
            return false;
        }


        magnetPurchased =
            false;


        return true;
    }


    // =====================================================
    // SPEED
    // =====================================================

    public bool IsSpeedPurchased()
    {
        return speedPurchased;
    }


    public bool ConsumeSpeed()
    {
        if (!speedPurchased)
        {
            return false;
        }


        speedPurchased =
            false;


        return true;
    }


    // =====================================================
    // REFRESH ALL UI
    // =====================================================

    private void RefreshAllUI()
    {
        RefreshStartingBrickUI();

        RefreshMagnetUI();

        RefreshSpeedUI();
    }


    // =====================================================
    // BRICK UI
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
            magnetBuyButton.interactable =
                !magnetPurchased;
        }
    }


    // =====================================================
    // SPEED UI
    // =====================================================

    private void RefreshSpeedUI()
    {
        if (speedStateText != null)
        {
            speedStateText.text =
                speedPurchased
                    ? "READY!"
                    : "READY: NO";
        }


        if (speedBuyButton != null)
        {
            speedBuyButton.interactable =
                !speedPurchased;
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