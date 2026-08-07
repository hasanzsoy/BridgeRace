using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] private TeamColor teamColor = TeamColor.Blue;
    public TeamColor CharacterTeamColor => teamColor;

    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 12f;

    protected Rigidbody rb;

    private Vector3 moveDirection;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        MoveCharacter();
        RotateCharacter();
    }

    protected void SetMoveDirection(Vector3 direction)
    {
        direction.y = 0f;

        moveDirection = direction.normalized;
    }

    private void MoveCharacter()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;

        rb.linearVelocity = velocity;
    }

    private void RotateCharacter()
    {
        if (moveDirection == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        Quaternion newRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation);
    }
}