using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class CharacterBase : MonoBehaviour, IRacer
{
    // =====================================================
    // CHARACTER SETTINGS
    // =====================================================

    [Header("Character Settings")]

    [SerializeField]
    private TeamColor teamColor = TeamColor.Blue;


    // =====================================================
    // MOVEMENT SETTINGS
    // =====================================================

    [Header("Movement Settings")]

    [SerializeField]
    protected float moveSpeed = 5f;

    [SerializeField]
    protected float rotationSpeed = 12f;


    // =====================================================
    // REFERENCES
    // =====================================================

    protected Rigidbody rb;

    protected CharacterBridgeBuilder bridgeBuilder;


    // =====================================================
    // MOVEMENT RUNTIME
    // =====================================================

    private Vector3 moveDirection;

    private bool movementEnabled = true;


    // =====================================================
    // PUBLIC VALUES
    // =====================================================

    public TeamColor CharacterTeamColor =>
        teamColor;


    public TeamColor RacerColor =>
        teamColor;


    // Speed Power-Up için mevcut hızı
    // dışarıdan okuyabiliriz.
    public float CurrentMoveSpeed =>
        moveSpeed;


    // =====================================================
    // AWAKE
    // =====================================================

    protected virtual void Awake()
    {
        rb =
            GetComponent<Rigidbody>();


        bridgeBuilder =
            GetComponent<CharacterBridgeBuilder>();
    }


    // =====================================================
    // FIXED UPDATE
    // =====================================================

    protected virtual void FixedUpdate()
    {
        if (!movementEnabled)
        {
            return;
        }


        // Hareket etmeden önce
        // köprü basamağını kontrol et.
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


    // =====================================================
    // MOVE DIRECTION
    // =====================================================

    protected void SetMoveDirection(
        Vector3 direction)
    {
        if (!movementEnabled)
        {
            moveDirection =
                Vector3.zero;

            return;
        }


        direction.y =
            0f;


        moveDirection =
            Vector3.ClampMagnitude(
                direction,
                1f
            );
    }


    // =====================================================
    // MOVEMENT ENABLE
    // =====================================================

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


    // =====================================================
    // MOVE SPEED
    // =====================================================

    public void SetMoveSpeed(
        float newSpeed)
    {
        moveSpeed =
            Mathf.Max(
                0f,
                newSpeed
            );
    }


    // =====================================================
    // ALLOWED MOVE DIRECTION
    // =====================================================

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


        blockedDirection.y =
            0f;


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


        // Yasak basamağa doğru olan
        // hareket bileşenini kaldır.
        if (movingTowardBlockedStep > 0f)
        {
            allowedDirection -=
                blockedDirection *
                movingTowardBlockedStep;
        }


        return allowedDirection;
    }


    // =====================================================
    // MOVE CHARACTER
    // =====================================================

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


    // =====================================================
    // ROTATE CHARACTER
    // =====================================================

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