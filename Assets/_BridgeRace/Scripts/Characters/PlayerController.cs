using UnityEngine;

public class PlayerController : CharacterBase
{
    [Header("Editor Test Settings")]
    [SerializeField] private bool keyboardTestEnabled = true;

    private Vector2 keyboardInput;
    private Vector2 mobileInput;

    private void Update()
    {
        ReadKeyboardInput();
        SendMovementToCharacter();
    }

    private void ReadKeyboardInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (keyboardTestEnabled)
        {
            float horizontalInput =
                Input.GetAxisRaw("Horizontal");

            float verticalInput =
                Input.GetAxisRaw("Vertical");

            keyboardInput = new Vector2(
                horizontalInput,
                verticalInput
            );

            keyboardInput = Vector2.ClampMagnitude(
                keyboardInput,
                1f
            );
        }
        else
        {
            keyboardInput = Vector2.zero;
        }
#else
        keyboardInput = Vector2.zero;
#endif
    }

    private void SendMovementToCharacter()
    {
        Vector2 selectedInput;

        if (mobileInput.sqrMagnitude > 0.001f)
        {
            selectedInput = mobileInput;
        }
        else
        {
            selectedInput = keyboardInput;
        }

        Vector3 movementDirection = new Vector3(
            selectedInput.x,
            0f,
            selectedInput.y
        );

        SetMoveDirection(movementDirection);
    }

    public void SetMobileMovementInput(Vector2 input)
    {
        mobileInput = Vector2.ClampMagnitude(
            input,
            1f
        );
    }

    public void StopMobileMovement()
    {
        mobileInput = Vector2.zero;
    }
}