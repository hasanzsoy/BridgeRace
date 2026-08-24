using DG.Tweening;
using UnityEngine;

public class VictoryPanelAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform victoryText;
    [SerializeField] private RectTransform continueButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private float textDuration = 0.45f;
    [SerializeField] private float buttonDuration = 0.35f;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();

            canvasGroup.alpha = 0f;

            canvasGroup
                .DOFade(1f, fadeDuration)
                .SetEase(Ease.OutQuad);
        }

        if (victoryText != null)
        {
            victoryText.DOKill();

            victoryText.localScale =
                Vector3.zero;

            victoryText
                .DOScale(
                    Vector3.one,
                    textDuration
                )
                .SetEase(Ease.OutBack);
        }

        if (continueButton != null)
        {
            continueButton.DOKill();

            continueButton.localScale =
                Vector3.zero;

            continueButton
                .DOScale(
                    Vector3.one,
                    buttonDuration
                )
                .SetDelay(0.25f)
                .SetEase(Ease.OutBack);
        }
    }
}