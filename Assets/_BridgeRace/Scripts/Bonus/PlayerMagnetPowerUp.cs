using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnetPowerUp : MonoBehaviour
{
    [Header("Magnet Settings")]

    [SerializeField]
    private float magnetRadius = 4f;

    [SerializeField]
    private float pullSpeed = 10f;

    [SerializeField]
    private float collectDistance = 0.55f;

    [SerializeField]
    private float scanInterval = 0.08f;

    [Header("Optional Visual")]

    [SerializeField]
    private GameObject magnetVisual;

    private PlayerController player;

    private bool magnetActive;

    private bool acceptingNewBricks;

    private float magnetEndTime;

    private float nextScanTime;


    // Magnet tarafından yakalanmış Brickler.
    private readonly List<Brick> attractedBricks =
        new List<Brick>();

    private readonly Collider[] overlapResults =
        new Collider[128];

    private void Awake()
    {
        player =
            GetComponent<PlayerController>();


        if (player == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde PlayerController bulunamadı!"
            );
        }


        if (magnetVisual != null)
        {
            magnetVisual.SetActive(
                false
            );
        }
    }


    private void Update()
    {
        if (!magnetActive)
        {
            return;
        }

        if (acceptingNewBricks &&
            Time.time >= magnetEndTime)
        {
            // Bundan sonra yeni Brick yakalama.
            acceptingNewBricks = false;


            if (magnetVisual != null)
            {
                magnetVisual.SetActive(
                    false
                );
            }
        }

        if (acceptingNewBricks &&
            Time.time >= nextScanTime)
        {
            nextScanTime =
                Time.time +
                scanInterval;


            ScanForBricks();
        }

        PullBricks();

        // Süre bitti ve çekilecek Brick kalmadı.
        if (!acceptingNewBricks &&
            attractedBricks.Count <= 0)
        {
            StopMagnet();
        }
    }

    public void ActivateMagnet(
        float duration)
    {
        if (duration <= 0f)
        {
            return;
        }


        if (player == null)
        {
            return;
        }


        attractedBricks.Clear();


        magnetActive =
            true;


        acceptingNewBricks =
            true;


        magnetEndTime =
            Time.time +
            duration;


        nextScanTime =
            0f;


        if (magnetVisual != null)
        {
            magnetVisual.SetActive(
                true
            );
        }


        Debug.Log(
            "MAGNET AKTİF! Süre: " +
            duration +
            " saniye"
        );
    }

    private void ScanForBricks()
    {
        if (player == null)
        {
            return;
        }

        int hitCount =
            Physics.OverlapSphereNonAlloc(
                transform.position,
                magnetRadius,
                overlapResults,
                ~0,
                QueryTriggerInteraction.Collide
            );


        for (int i = 0;
             i < hitCount;
             i++)
        {
            Collider hitCollider =
                overlapResults[i];


            if (hitCollider == null)
            {
                continue;
            }

            if (!hitCollider.TryGetComponent<ICollectable>(
                    out ICollectable collectable))
            {
                continue;
            }


            // Bu Magnet yalnızca gerçek Brick objelerini
            // fiziksel olarak hareket ettirecek.
            Brick brick =
                collectable as Brick;


            if (brick == null)
            {
                continue;
            }


            // Brick hâlâ alınabilir mi?
            if (!brick.CanBeCollected)
            {
                continue;
            }

            if (brick.CollectableColor !=
                player.CharacterTeamColor)
            {
                continue;
            }


            // Daha önce listeye aldıysak tekrar ekleme.
            if (attractedBricks.Contains(
                    brick))
            {
                continue;
            }


            attractedBricks.Add(
                brick
            );
        }
    }


    private void PullBricks()
    {
        if (player == null)
        {
            return;
        }


        Vector3 targetPosition =
            player.transform.position +
            Vector3.up * 0.5f;


        // Liste içinden silme yapacağımız için
        // sondan başa doğru gidiyoruz.
        for (int i =
                 attractedBricks.Count - 1;
             i >= 0;
             i--)
        {
            Brick brick =
                attractedBricks[i];


            // Brick yok oldu / pool'a döndü.
            if (brick == null ||
                !brick.gameObject.activeInHierarchy)
            {
                attractedBricks.RemoveAt(
                    i
                );

                continue;
            }


            // Başka şekilde toplanmış olabilir.
            if (!brick.CanBeCollected)
            {
                attractedBricks.RemoveAt(
                    i
                );

                continue;
            }


            // Güvenlik:
            // rengi artık uygun değilse bırak.
            if (brick.CollectableColor !=
                player.CharacterTeamColor)
            {
                attractedBricks.RemoveAt(
                    i
                );

                continue;
            }

            brick.transform.position =
                Vector3.MoveTowards(
                    brick.transform.position,
                    targetPosition,
                    pullSpeed *
                    Time.deltaTime
                );

            float distanceSquared =
                (
                    brick.transform.position -
                    targetPosition
                ).sqrMagnitude;


            if (distanceSquared >
                collectDistance *
                collectDistance)
            {
                continue;
            }

            brick.Collect(
                player
            );


            attractedBricks.RemoveAt(
                i
            );
        }
    }


    private void StopMagnet()
    {
        magnetActive =
            false;


        acceptingNewBricks =
            false;


        attractedBricks.Clear();


        if (magnetVisual != null)
        {
            magnetVisual.SetActive(
                false
            );
        }


        Debug.Log(
            "MAGNET BİTTİ!"
        );
    }


    // =====================================================
    // GIZMO
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            magnetRadius
        );
    }
}