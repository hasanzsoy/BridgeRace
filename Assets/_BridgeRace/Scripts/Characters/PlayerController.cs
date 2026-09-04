using UnityEngine;

public class PlayerController : CharacterBase
{
    [Header("Mobile Input")]
    [SerializeField] private DynamicJoystick dynamicJoystick;

    [Header("Keyboard Input")]
    [SerializeField] private bool keyboardTestEnabled = true;
         
    private void Update()
    {
        Vector2 input = Vector2.zero;
                
        if (dynamicJoystick != null)
        {
            input = dynamicJoystick.InputDirection;
        }


#if UNITY_EDITOR || UNITY_WEBGL

        if (input == Vector2.zero &&keyboardTestEnabled)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            float verticalInput = Input.GetAxisRaw("Vertical");

            input = new Vector2(horizontalInput,verticalInput);
        }

#endif

        Vector3 direction = new Vector3(input.x,0f,input.y);

        SetMoveDirection(direction);
    }
}