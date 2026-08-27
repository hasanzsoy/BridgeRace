using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    // =====================================================
    // UI REFERENCES
    // =====================================================

    [Header("UI References")]

    [SerializeField]
    private TMP_Text comboText;

    [SerializeField]
    private GameObject comboRewardRoot;

    [SerializeField]
    private TMP_Text comboRewardText;


    // =====================================================
    // ANIMATION SETTINGS
    // =====================================================

    [Header("Animation Settings")]

    [SerializeField]
    private float punchScale = 0.18f;

    [SerializeField]
    private float punchDuration = 0.20f;

    [SerializeField]
    private float rewardShowDuration = 0.8f;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        // Oyun başlarken ödül paneli görünmesin.
        //
        // Böylece:
        // - zemin
        // - COMBO! +2 yazısı
        //
        // birlikte gizlenmiş olur.

        HideComboReward();
    }


    // =====================================================
    // EVENTS
    // =====================================================

    private void OnEnable()
    {
        EventManager.OnComboChanged +=
            UpdateComboText;

        EventManager.OnComboCompleted +=
            ShowComboReward;
    }


    private void OnDisable()
    {
        EventManager.OnComboChanged -=
            UpdateComboText;

        EventManager.OnComboCompleted -=
            ShowComboReward;


        // Script kapanırken yarım kalan
        // tweenleri temizle.

        if (comboText != null)
        {
            comboText.transform.DOKill();
        }


        if (comboRewardRoot != null)
        {
            comboRewardRoot.transform.DOKill();
        }
    }


    // =====================================================
    // NORMAL COMBO TEXT
    // =====================================================

    private void UpdateComboText(
        int currentCombo,
        int requiredCombo)
    {
        if (comboText == null)
        {
            return;
        }


        comboText.text =
            "COMBO " +
            currentCombo +
            "/" +
            requiredCombo;


        // Önce eski animation varsa durdur.
        comboText.transform.DOKill();


        // Scale'i sıfırla.
        comboText.transform.localScale =
            Vector3.one;


        // Her brick alındığında
        // ufak punch animation.
        comboText.transform.DOPunchScale(
            Vector3.one * punchScale,
            punchDuration,
            5,
            0.5f
        );
    }


    // =====================================================
    // COMBO REWARD
    // =====================================================

    private void ShowComboReward(
        int bonusBrickAmount)
    {
        if (comboRewardRoot == null ||
            comboRewardText == null)
        {
            return;
        }


        // Önce eski tween varsa temizle.
        comboRewardRoot.transform.DOKill();


        // Yazıyı güncelle.
        comboRewardText.text =
            "COMBO! +" +
            bonusBrickAmount;


        // =================================================
        // ÖDÜLÜ AÇ
        //
        // Burada artık sadece yazıyı değil,
        // bütün ComboRewardRoot'u açıyoruz.
        //
        // Yani:
        // ZEMİN + YAZI
        // birlikte açılacak.
        // =================================================

        comboRewardRoot.SetActive(
            true
        );


        // Animation başlangıcında görünmez boyut.
        comboRewardRoot.transform.localScale =
            Vector3.zero;


        // =================================================
        // DOTWEEN SEQUENCE
        // =================================================

        Sequence sequence =
            DOTween.Sequence();


        // İlk açılış.
        sequence.Append(
            comboRewardRoot.transform
                .DOScale(
                    1.25f,
                    0.20f
                )
                .SetEase(
                    Ease.OutBack
                )
        );


        // Hafif küçülerek normal boyuta gelsin.
        sequence.Append(
            comboRewardRoot.transform
                .DOScale(
                    1f,
                    0.10f
                )
        );


        // Bir süre ekranda kalsın.
        sequence.AppendInterval(
            rewardShowDuration
        );


        // Kapanış.
        sequence.Append(
            comboRewardRoot.transform
                .DOScale(
                    0f,
                    0.20f
                )
                .SetEase(
                    Ease.InBack
                )
        );


        // Animation bitince
        // bütün Root'u kapat.
        sequence.OnComplete(
            HideComboReward
        );
    }


    // =====================================================
    // HIDE REWARD
    // =====================================================

    private void HideComboReward()
    {
        if (comboRewardRoot == null)
        {
            return;
        }


        comboRewardRoot.transform.DOKill();


        comboRewardRoot.transform.localScale =
            Vector3.one;


        comboRewardRoot.SetActive(
            false
        );
    }
}