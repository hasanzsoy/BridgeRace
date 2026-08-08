using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] private TeamColor teamColor = TeamColor.Blue;

    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 12f;

    protected Rigidbody rb;

    private Vector3 moveDirection;

    private bool movementEnabled = true;


    public TeamColor CharacterTeamColor => teamColor;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    protected virtual void FixedUpdate()
    {
        if (!movementEnabled)
        {
            return;
        }

        MoveCharacter();
        RotateCharacter();
    }


    protected void SetMoveDirection(Vector3 direction)
    {
        if (!movementEnabled)
        {
            moveDirection = Vector3.zero;
            return;
        }

        direction.y = 0f;

        moveDirection = Vector3.ClampMagnitude(
            direction,
            1f
        );
    }


    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!movementEnabled)
        {
            moveDirection = Vector3.zero;
        }
    }


    private void MoveCharacter()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.x =
            moveDirection.x * moveSpeed;

        velocity.z =
            moveDirection.z * moveSpeed;

        rb.linearVelocity = velocity;
    }


    private void RotateCharacter()
    {
        if (moveDirection == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(
                moveDirection
            );

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed *
                Time.fixedDeltaTime
            );

        rb.MoveRotation(newRotation);
    }
}