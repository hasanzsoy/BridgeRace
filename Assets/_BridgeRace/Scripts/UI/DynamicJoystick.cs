using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("Joystick References")]
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;

    [Header("Joystick Settings")]
    [SerializeField] private float handleLimit = 0.6f;

    private Canvas canvas;
    private RectTransform canvasRect;

    private Vector2 inputDirection;

    public Vector2 InputDirection => inputDirection;


    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            canvasRect = canvas.transform as RectTransform;
        }

        joystickBackground.gameObject.SetActive(false);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        ShowJoystick(eventData);

        OnDrag(eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        float radius = joystickBackground.rect.width / 2f;

        inputDirection = localPoint / radius;

        inputDirection = Vector2.ClampMagnitude(
            inputDirection,
            1f
        );

        joystickHandle.anchoredPosition =
            inputDirection *
            radius *
            handleLimit;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        inputDirection = Vector2.zero;

        joystickHandle.anchoredPosition =
            Vector2.zero;

        joystickBackground.gameObject.SetActive(false);
    }


    private void ShowJoystick(PointerEventData eventData)
    {
        joystickBackground.gameObject.SetActive(true);

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        joystickBackground.anchoredPosition =
            localPoint;

        joystickHandle.anchoredPosition =
            Vector2.zero;
    }


    private void OnDisable()
    {
        inputDirection = Vector2.zero;

        if (joystickHandle != null)
        {
            joystickHandle.anchoredPosition =
                Vector2.zero;
        }

        if (joystickBackground != null)
        {
            joystickBackground.gameObject.SetActive(false);
        }
    }
}