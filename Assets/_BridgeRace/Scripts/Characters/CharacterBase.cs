using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class CharacterBase :MonoBehaviour,IRacer
{
    [Header("Character Settings")]
    [SerializeField] private TeamColor teamColor = TeamColor.Blue;

    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 12f;

    protected Rigidbody rb;

    private Vector3 moveDirection;

    private bool movementEnabled = true;

    private CharacterBridgeBuilder bridgeBuilder;


    public TeamColor CharacterTeamColor => teamColor;
    public TeamColor RacerColor => teamColor;



    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        bridgeBuilder =
            GetComponent<CharacterBridgeBuilder>();
    }


    protected virtual void FixedUpdate()
    {
        if (!movementEnabled)
        {
            return;
        }


        // Hareket etmeden önce önümüzdeki
        // köprü basamağını kontrol ediyoruz.
        if (bridgeBuilder != null)
        {
            bridgeBuilder.RefreshBridgeCheck();
        }


        Vector3 allowedDirection =
            GetAllowedMoveDirection();


        MoveCharacter(
            allowedDirection
        );


        RotateCharacter(
            allowedDirection
        );
    }


    protected void SetMoveDirection(
        Vector3 direction)
    {
        if (!movementEnabled)
        {
            moveDirection =
                Vector3.zero;

            return;
        }


        direction.y = 0f;


        moveDirection =
            Vector3.ClampMagnitude(
                direction,
                1f
            );
    }


    public void SetMovementEnabled(
        bool enabled)
    {
        movementEnabled =
            enabled;


        if (!movementEnabled)
        {
            moveDirection =
                Vector3.zero;
        }
    }


    private Vector3 GetAllowedMoveDirection()
    {
        Vector3 allowedDirection =
            moveDirection;


        if (bridgeBuilder == null)
        {
            return allowedDirection;
        }


        if (!bridgeBuilder.IsForwardBlocked)
        {
            return allowedDirection;
        }


        Vector3 blockedDirection =
            bridgeBuilder.BlockedDirection;


        blockedDirection.y = 0f;


        if (blockedDirection.sqrMagnitude <
            0.001f)
        {
            return allowedDirection;
        }


        blockedDirection.Normalize();


        float movingTowardBlockedStep =
            Vector3.Dot(
                allowedDirection,
                blockedDirection
            );


        // Karakter yasak olan basamağa
        // doğru hareket ediyorsa,
        // o yöndeki hareketi kaldırıyoruz.
        if (movingTowardBlockedStep > 0f)
        {
            allowedDirection -=
                blockedDirection *
                movingTowardBlockedStep;
        }


        return allowedDirection;
    }


    private void MoveCharacter(
        Vector3 allowedDirection)
    {
        Vector3 velocity =
            rb.linearVelocity;


        velocity.x =
            allowedDirection.x *
            moveSpeed;


        velocity.z =
            allowedDirection.z *
            moveSpeed;


        rb.linearVelocity =
            velocity;
    }


    private void RotateCharacter(
        Vector3 allowedDirection)
    {
        if (allowedDirection ==
            Vector3.zero)
        {
            return;
        }


        Quaternion targetRotation =
            Quaternion.LookRotation(
                allowedDirection
            );


        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed *
                Time.fixedDeltaTime
            );


        rb.MoveRotation(
            newRotation
        );
    }
}