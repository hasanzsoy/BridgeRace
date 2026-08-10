using UnityEngine;

public class CharacterBridgeBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rayOrigin;

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 1.5f;


    private CharacterBase character;

    private CharacterStack characterStack;


    public bool IsForwardBlocked
    {
        get;
        private set;
    }


    public Vector3 BlockedDirection
    {
        get;
        private set;
    }


    private void Awake()
    {
        character =
            GetComponent<CharacterBase>();


        characterStack =
            GetComponent<CharacterStack>();


        if (character == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterBase bulunamadı!"
            );
        }


        if (characterStack == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterStack bulunamadı!"
            );
        }


        if (rayOrigin == null)
        {
            Debug.LogError(
                gameObject.name +
                " için BuildRayOrigin atanmadı!"
            );
        }
    }


    public void RefreshBridgeCheck()
    {
        // Her kontrolde önce engeli temizliyoruz.
        IsForwardBlocked = false;

        BlockedDirection =
            Vector3.zero;


        if (rayOrigin == null)
        {
            return;
        }


        Debug.DrawRay(
            rayOrigin.position,
            Vector3.down *
            rayDistance,
            Color.red
        );


        if (Physics.Raycast(
                rayOrigin.position,
                Vector3.down,
                out RaycastHit hit,
                rayDistance,
                ~0,
                QueryTriggerInteraction.Collide))
        {
            if (hit.collider.TryGetComponent<IBuildable>(
                    out IBuildable buildable))
            {
                CheckBuildable(
                    buildable,
                    hit.point
                );
            }
        }
    }


    private void CheckBuildable(
        IBuildable buildable,
        Vector3 hitPoint)
    {
        if (character == null ||
            characterStack == null)
        {
            return;
        }


        TeamColor characterColor =
            character.CharacterTeamColor;


        // Step zaten bizim rengimizse
        // rahatça yürüyebiliriz.
        if (!buildable.NeedsBuild(
                characterColor))
        {
            return;
        }


        // Rakip Step veya boş Step var.
        // Brick yoksa ileri hareketi engelle.
        if (characterStack.BrickCount <= 0)
        {
            BlockMovement(
                hitPoint
            );

            return;
        }


        // Brick harcamayı dene.
        bool brickSpent =
            characterStack.TrySpendBrick();


        if (!brickSpent)
        {
            BlockMovement(
                hitPoint
            );

            return;
        }


        // Brick varsa Step'i
        // kendi rengimize çevir.
        buildable.BuildStep(
            characterColor
        );
    }


    private void BlockMovement(
        Vector3 hitPoint)
    {
        Vector3 direction =
            hitPoint -
            transform.position;


        direction.y = 0f;


        if (direction.sqrMagnitude <
            0.001f)
        {
            direction =
                transform.forward;
        }


        IsForwardBlocked =
            true;


        BlockedDirection =
            direction.normalized;
    }


    private void OnDisable()
    {
        IsForwardBlocked =
            false;

        BlockedDirection =
            Vector3.zero;
    }
}