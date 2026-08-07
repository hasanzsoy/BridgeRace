using UnityEngine;

public class PlayerController : CharacterBase
{
    private float horizontalInput;
    private float verticalInput;

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(
            horizontalInput,
            0f,
            verticalInput
        );

        SetMoveDirection(direction);
    }
}