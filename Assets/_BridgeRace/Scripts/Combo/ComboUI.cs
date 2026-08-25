using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text comboRewardText;

    [Header("Animation Settings")]
    [SerializeField] private float punchScale = 0.18f;
    [SerializeField] private float punchDuration = 0.20f;

    [SerializeField] private float rewardShowDuration = 0.8f;


    private void Awake()
    {
        if (comboRewardText != null)
        {
            comboRewardText.gameObject.SetActive(
                false
            );
        }
    }


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
    }


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


        comboText.transform.DOKill();


        comboText.transform.localScale =
            Vector3.one;


        comboText.transform.DOPunchScale(
            Vector3.one * punchScale,
            punchDuration,
            5,
            0.5f
        );
    }


    private void ShowComboReward(
        int bonusBrickAmount)
    {
        if (comboRewardText == null)
        {
            return;
        }


        comboRewardText.gameObject.SetActive(
            true
        );


        comboRewardText.text =
            "COMBO! +" +
            bonusBrickAmount;


        comboRewardText.transform.DOKill();


        comboRewardText.transform.localScale =
            Vector3.zero;


        Sequence sequence =
            DOTween.Sequence();


        sequence.Append(
            comboRewardText.transform
                .DOScale(
                    1.25f,
                    0.20f
                )
                .SetEase(
                    Ease.OutBack
                )
        );


        sequence.Append(
            comboRewardText.transform
                .DOScale(
                    1f,
                    0.10f
                )
        );


        sequence.AppendInterval(
            rewardShowDuration
        );


        sequence.Append(
            comboRewardText.transform
                .DOScale(
                    0f,
                    0.20f
                )
                .SetEase(
                    Ease.InBack
                )
        );


        sequence.OnComplete(
            () =>
            {
                comboRewardText.gameObject
                    .SetActive(false);
            }
        );
    }
}