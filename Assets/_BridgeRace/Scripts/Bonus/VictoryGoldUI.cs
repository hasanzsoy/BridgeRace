using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class VictoryGoldUI : MonoBehaviour
{
    [Header("Gold Section")]
    [SerializeField]
    private GameObject goldSection;


    [Header("Center Reward")]
    [SerializeField]
    private RectTransform rewardRoot;

    [SerializeField]
    private TMP_Text rewardGoldText;


    [Header("Flying Coins")]
    [SerializeField]
    private RectTransform flyCoinsRoot;


    [Header("Top Right Wallet")]
    [SerializeField]
    private RectTransform walletPanel;

    [SerializeField]
    private TMP_Text walletGoldText;

    [SerializeField]
    private RectTransform walletFlyTarget;


    [Header("Reference Style Timing")]

    [SerializeField]
    private float startDelay = 0.20f;

    [SerializeField]
    private float rewardPopDuration = 0.22f;

    [SerializeField]
    private float rewardHoldDuration = 0.75f;

    [SerializeField]
    private float rewardHideDuration = 0.15f;

    [SerializeField]
    private float pilePopDuration = 0.22f;

    [SerializeField]
    private float pileHoldDuration = 0.40f;

    [SerializeField]
    private float coinFlyDuration = 0.60f;

    [SerializeField]
    private float coinFlyDelay = 0.09f;


    [Header("Wallet Animation")]

    [SerializeField]
    private float walletPunchScale = 0.10f;

    [SerializeField]
    private float walletPunchDuration = 0.18f;


    private RectTransform[] flyingCoins;

    private Vector2[] originalCoinPositions;

    private Coroutine rewardCoroutine;


    private int oldGold;

    private int rewardGold;

    private int finalGold;


    private void Awake()
    {
        PrepareFlyingCoins();


        if (rewardRoot != null)
        {
            rewardRoot.localScale =
                Vector3.zero;
        }
    }


    private void OnEnable()
    {
        EventManager.OnVictoryGoldReward +=
            OnVictoryGoldReward;
    }


    private void OnDisable()
    {
        EventManager.OnVictoryGoldReward -=
            OnVictoryGoldReward;
    }


    // =====================================================
    // PREPARE COINS
    // =====================================================

    private void PrepareFlyingCoins()
    {
        if (flyCoinsRoot == null)
        {
            return;
        }


        int coinCount =
            flyCoinsRoot.childCount;


        flyingCoins =
            new RectTransform[coinCount];


        originalCoinPositions =
            new Vector2[coinCount];


        for (int i = 0;
             i < coinCount;
             i++)
        {
            RectTransform coin =
                flyCoinsRoot
                    .GetChild(i)
                    .GetComponent<RectTransform>();


            flyingCoins[i] =
                coin;


            if (coin != null)
            {
                originalCoinPositions[i] =
                    coin.anchoredPosition;


                coin.gameObject.SetActive(
                    false
                );
            }
        }
    }


    // =====================================================
    // EVENT
    // =====================================================

    private void OnVictoryGoldReward(
        int previousGold,
        int earnedGold,
        int newGold)
    {
        oldGold =
            previousGold;

        rewardGold =
            earnedGold;

        finalGold =
            newGold;


        if (rewardCoroutine != null)
        {
            StopCoroutine(
                rewardCoroutine
            );
        }


        rewardCoroutine =
            StartCoroutine(
                WaitForVictoryPanel()
            );
    }


    // =====================================================
    // WAIT PANEL
    // =====================================================

    private IEnumerator WaitForVictoryPanel()
    {
        if (goldSection != null)
        {
            goldSection.SetActive(
                true
            );
        }


        // VictoryPanel başka script tarafından
        // aynı frame veya biraz sonra açılabilir.
        //
        // Panel görünür olana kadar bekliyoruz.

        while (goldSection != null &&
               !goldSection.activeInHierarchy)
        {
            yield return null;
        }


        yield return null;


        PlayGoldAnimation();
    }


    // =====================================================
    // MAIN ANIMATION
    // =====================================================

    private void PlayGoldAnimation()
    {
        ResetVisuals();


        if (walletGoldText != null)
        {
            walletGoldText.text =
                oldGold.ToString();
        }


        if (rewardGoldText != null)
        {
            rewardGoldText.text =
                rewardGold.ToString();
        }


        Sequence sequence =
            DOTween.Sequence();


        // Victory ekranında Time.timeScale 0 olsa bile
        // UI animasyonları devam etsin.
        sequence.SetUpdate(
            true
        );


        sequence.AppendInterval(
            startDelay
        );


        // =========================================
        // 1. ÖDÜL COIN + SAYI POP
        // =========================================

        if (rewardRoot != null)
        {
            sequence.Append(
                rewardRoot
                    .DOScale(
                        1.15f,
                        rewardPopDuration
                    )
                    .SetEase(
                        Ease.OutBack
                    )
            );


            sequence.Append(
                rewardRoot
                    .DOScale(
                        1f,
                        0.10f
                    )
            );
        }


        sequence.AppendInterval(
            rewardHoldDuration
        );


        // =========================================
        // 2. ORTADAKİ ÖDÜL KÜÇÜLÜR
        // =========================================

        if (rewardRoot != null)
        {
            sequence.Append(
                rewardRoot
                    .DOScale(
                        0f,
                        rewardHideDuration
                    )
                    .SetEase(
                        Ease.InBack
                    )
            );
        }


        // =========================================
        // 3. COIN YIĞINI ÇIKAR
        // =========================================

        sequence.AppendCallback(
            ShowCoinPile
        );


        sequence.AppendInterval(
            pilePopDuration +
            pileHoldDuration
        );


        // =========================================
        // 4. COINLER CÜZDANA UÇAR
        // =========================================

        sequence.AppendCallback(
            StartFlyingCoins
        );
    }


    // =====================================================
    // RESET
    // =====================================================

    private void ResetVisuals()
    {
        if (rewardRoot != null)
        {
            rewardRoot.DOKill();

            rewardRoot.localScale =
                Vector3.zero;
        }


        if (walletPanel != null)
        {
            walletPanel.DOKill();

            walletPanel.localScale =
                Vector3.one;
        }


        if (flyingCoins == null)
        {
            return;
        }


        for (int i = 0;
             i < flyingCoins.Length;
             i++)
        {
            RectTransform coin =
                flyingCoins[i];


            if (coin == null)
            {
                continue;
            }


            coin.DOKill();


            coin.anchoredPosition =
                originalCoinPositions[i];


            coin.localScale =
                Vector3.zero;


            coin.gameObject.SetActive(
                false
            );
        }
    }


    // =====================================================
    // COIN PILE
    // =====================================================

    private void ShowCoinPile()
    {
        if (flyingCoins == null)
        {
            return;
        }


        for (int i = 0;
             i < flyingCoins.Length;
             i++)
        {
            RectTransform coin =
                flyingCoins[i];


            if (coin == null)
            {
                continue;
            }


            coin.gameObject.SetActive(
                true
            );


            coin.localScale =
                Vector3.zero;


            coin
                .DOScale(
                    1f,
                    pilePopDuration
                )
                .SetDelay(
                    i * 0.025f
                )
                .SetEase(
                    Ease.OutBack
                )
                .SetUpdate(
                    true
                );
        }
    }


    // =====================================================
    // START FLY
    // =====================================================

    private void StartFlyingCoins()
    {
        if (flyingCoins == null ||
            flyingCoins.Length == 0 ||
            walletFlyTarget == null)
        {
            if (walletGoldText != null)
            {
                walletGoldText.text =
                    finalGold.ToString();
            }

            return;
        }


        for (int i = 0;
             i < flyingCoins.Length;
             i++)
        {
            int coinIndex =
                i;


            DOVirtual.DelayedCall(
                coinFlyDelay * i,
                () =>
                {
                    FlySingleCoin(
                        coinIndex
                    );
                },
                true
            );
        }
    }


    // =====================================================
    // SINGLE COIN
    // =====================================================

    private void FlySingleCoin(
        int coinIndex)
    {
        if (coinIndex < 0 ||
            coinIndex >= flyingCoins.Length)
        {
            return;
        }


        RectTransform coin =
            flyingCoins[
                coinIndex
            ];


        if (coin == null)
        {
            return;
        }


        Vector3 startPosition =
            coin.position;


        Vector3 endPosition =
            walletFlyTarget.position;


        // Referanstaki gibi bütün coinler aynı çizgiden
        // gitmesin. Hafif farklı kavisler oluşturalım.

        Vector3 middlePosition =
            Vector3.Lerp(
                startPosition,
                endPosition,
                0.55f
            );


        middlePosition +=
            new Vector3(
                Random.Range(
                    -65f,
                    65f
                ),
                Random.Range(
                    -20f,
                    70f
                ),
                0f
            );


        Vector3[] path =
        {
            middlePosition,
            endPosition
        };


        Sequence flySequence =
            DOTween.Sequence();


        flySequence.SetUpdate(
            true
        );


        flySequence.Join(
            coin.DOPath(
                    path,
                    coinFlyDuration,
                    PathType.CatmullRom
                )
                .SetEase(
                    Ease.InQuad
                )
        );


        flySequence.Join(
            coin.DOScale(
                0.65f,
                coinFlyDuration
            )
        );


        flySequence.OnComplete(
            () =>
            {
                OnCoinReachedWallet(
                    coinIndex,
                    coin
                );
            }
        );
    }


    // =====================================================
    // WALLET
    // =====================================================

    private void OnCoinReachedWallet(
        int coinIndex,
        RectTransform coin)
    {
        coin.gameObject.SetActive(
            false
        );


        int arrivedCoinCount =
            coinIndex + 1;


        int totalCoinCount =
            flyingCoins.Length;


        // Örneğin ödül 124 Gold ise,
        // 10 görsel coin 124 Gold'u parça parça temsil eder.

        float progress =
            (float)arrivedCoinCount /
            totalCoinCount;


        int displayedGold =
            oldGold +
            Mathf.RoundToInt(
                rewardGold *
                progress
            );


        if (arrivedCoinCount >=
            totalCoinCount)
        {
            displayedGold =
                finalGold;
        }


        if (walletGoldText != null)
        {
            walletGoldText.text =
                displayedGold.ToString();
        }


        // Coin cüzdana çarptığında
        // referanstaki küçük pop hissi.

        if (walletPanel != null)
        {
            walletPanel.DOKill(
                true
            );


            walletPanel.localScale =
                Vector3.one;


            walletPanel
                .DOPunchScale(
                    Vector3.one *
                    walletPunchScale,
                    walletPunchDuration,
                    5,
                    0.5f
                )
                .SetUpdate(
                    true
                );
        }
    }
}