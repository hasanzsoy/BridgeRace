using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressAnimation : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float pressedScale = 0.92f;
    [SerializeField] private float pressDuration = 0.08f;
    [SerializeField] private float releaseDuration = 0.15f;

    private RectTransform rectTransform;
    private Vector3 originalScale;


    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        originalScale =
            rectTransform.localScale;
    }


    public void OnPointerDown(
        PointerEventData eventData)
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(
                originalScale * pressedScale,
                pressDuration
            )
            .SetEase(Ease.OutQuad);
    }


    public void OnPointerUp(
        PointerEventData eventData)
    {
        ReleaseButton();
    }


    public void OnPointerExit(
        PointerEventData eventData)
    {
        ReleaseButton();
    }


    private void ReleaseButton()
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(
                originalScale,
                releaseDuration
            )
            .SetEase(Ease.OutBack);
    }


    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.DOKill();

            rectTransform.localScale =
                originalScale;
        }
    }
}