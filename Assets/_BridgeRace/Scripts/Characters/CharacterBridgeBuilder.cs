using UnityEngine;

public class CharacterBridgeBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rayOrigin;

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 1.5f;

    private CharacterBase character;
    private CharacterStack characterStack;


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


    private void FixedUpdate()
    {
        CheckBridgeStep();
    }


    private void CheckBridgeStep()
    {
        if (rayOrigin == null)
        {
            return;
        }


        Debug.DrawRay(
            rayOrigin.position,
            Vector3.down * rayDistance,
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
                TryBuild(
                    buildable
                );
            }
        }
    }


    private void TryBuild(
        IBuildable buildable)
    {
        if (character == null ||
            characterStack == null)
        {
            return;
        }


        TeamColor characterColor =
            character.CharacterTeamColor;


        if (!buildable.NeedsBuild(
                characterColor))
        {
            return;
        }


        bool brickSpent =
            characterStack.TrySpendBrick();


        if (!brickSpent)
        {
            return;
        }


        buildable.BuildStep(
            characterColor
        );
    }
}