using UnityEngine;

public class CharacterBridgeBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rayOrigin;

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 1.5f;


    private CharacterBase character;
    private CharacterStack characterStack;

    private bool bridgeCheckEnabled = true;


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
        IsForwardBlocked = false;

        BlockedDirection =
            Vector3.zero;


        if (!bridgeCheckEnabled)
        {
            return;
        }


        if (rayOrigin == null)
        {
            return;
        }


        Debug.DrawRay(
            rayOrigin.position,
            Vector3.down * rayDistance,
            Color.red
        );


        // ==========================================
        // ÖNEMLİ:
        //
        // Tek Raycast kullanmıyoruz.
        //
        // PhysicsRamp, Trigger veya başka Collider
        // ray'in önüne girse bile bütün hitleri
        // kontrol ediyoruz.
        // ==========================================

        RaycastHit[] hits =
            Physics.RaycastAll(
                rayOrigin.position,
                Vector3.down,
                rayDistance,
                ~0,
                QueryTriggerInteraction.Collide
            );


        if (hits == null ||
            hits.Length <= 0)
        {
            return;
        }


        IBuildable closestBuildable =
            null;


        Vector3 closestHitPoint =
            Vector3.zero;


        float closestDistance =
            Mathf.Infinity;


        // ==========================================
        // Sadece IBuildable olan collider'ları ara.
        //
        // PhysicsRamp_Final → geç
        // ActivationZone    → geç
        // Modifier Trigger  → geç
        // BridgeStep        → kullan
        // ==========================================

        for (int i = 0;
             i < hits.Length;
             i++)
        {
            RaycastHit hit =
                hits[i];


            if (!hit.collider.TryGetComponent<IBuildable>(
                    out IBuildable buildable))
            {
                continue;
            }


            if (hit.distance >=
                closestDistance)
            {
                continue;
            }


            closestDistance =
                hit.distance;


            closestBuildable =
                buildable;


            closestHitPoint =
                hit.point;
        }


        // Altımızda IBuildable yok.
        if (closestBuildable == null)
        {
            return;
        }


        CheckBuildable(
            closestBuildable,
            closestHitPoint
        );
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


        // Step zaten karakterin rengindeyse
        // tekrar Brick harcama.
        if (!buildable.NeedsBuild(
                characterColor))
        {
            return;
        }


        // Brick yoksa ilerlemeyi engelle.
        if (characterStack.BrickCount <= 0)
        {
            BlockMovement(
                hitPoint
            );

            return;
        }


        // Bir Brick harca.
        bool brickSpent =
            characterStack.TrySpendBrick();


        if (!brickSpent)
        {
            BlockMovement(
                hitPoint
            );

            return;
        }


        // Step'i karakterin rengine build et.
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


        direction.y =
            0f;


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


    public void SetBridgeCheckEnabled(
        bool enabled)
    {
        bridgeCheckEnabled =
            enabled;


        if (!enabled)
        {
            IsForwardBlocked =
                false;


            BlockedDirection =
                Vector3.zero;
        }
    }


    private void OnDisable()
    {
        IsForwardBlocked =
            false;


        BlockedDirection =
            Vector3.zero;
    }
}