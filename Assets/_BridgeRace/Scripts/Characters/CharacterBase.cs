using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class CharacterBase : MonoBehaviour
{
    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    [Header("Character Settings")]
    [SerializeField] private TeamColor teamColor = TeamColor.Blue;

    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 12f;

    [Header("Animation Settings")]
    [SerializeField] protected Animator characterAnimator;

    protected Rigidbody characterRigidbody;

    private Vector3 moveDirection;

    public TeamColor CharacterTeamColor => teamColor;

    public Vector3 MoveDirection => moveDirection;

    public bool IsMoving =>
        moveDirection.sqrMagnitude > 0.001f;

    protected virtual void Awake()
    {
        characterRigidbody = GetComponent<Rigidbody>();

        if (characterAnimator == null)
        {
            characterAnimator = GetComponentInChildren<Animator>();
        }

        if (characterAnimator == null)
        {
            Debug.LogError(
                $"{gameObject.name} nesnesinde Animator bulunamadı!",
                gameObject
            );
        }
    }

    protected virtual void FixedUpdate()
    {
        MoveCharacter();
        RotateCharacter();
    }

    protected void SetMoveDirection(Vector3 direction)
    {
        direction.y = 0f;

        moveDirection = Vector3.ClampMagnitude(direction, 1f);

        UpdateMovementAnimation();
    }

    private void MoveCharacter()
    {
        Vector3 currentVelocity =
            characterRigidbody.linearVelocity;

        Vector3 targetVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            currentVelocity.y,
            moveDirection.z * moveSpeed
        );

        characterRigidbody.linearVelocity = targetVelocity;
    }

    private void RotateCharacter()
    {
        if (!IsMoving)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(
            moveDirection,
            Vector3.up
        );

        Quaternion smoothRotation = Quaternion.Slerp(
            characterRigidbody.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        characterRigidbody.MoveRotation(smoothRotation);
    }

    private void UpdateMovementAnimation()
    {
        if (characterAnimator == null)
        {
            return;
        }

        characterAnimator.SetBool(
            IsMovingHash,
            IsMoving
        );
    }

    protected virtual void OnDisable()
    {
        moveDirection = Vector3.zero;

        if (characterAnimator != null)
        {
            characterAnimator.SetBool(
                IsMovingHash,
                false
            );
        }
    }
}