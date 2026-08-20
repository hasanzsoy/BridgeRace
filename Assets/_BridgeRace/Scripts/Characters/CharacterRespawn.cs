using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private Transform startingRespawnPoint;

    [Header("Fall Safety")]
    [SerializeField] private float fallYThreshold = -8f;
    [SerializeField] private float respawnVerticalOffset = 0.15f;


    private Transform currentRespawnPoint;

    private Rigidbody rb;

    private AIController aiController;

    private bool isRespawning;


    private void Awake()
    {
        rb =
            GetComponent<Rigidbody>();


        aiController =
            GetComponent<AIController>();


        currentRespawnPoint =
            startingRespawnPoint;


        if (startingRespawnPoint == null)
        {
            Debug.LogError(
                gameObject.name +
                " için Starting Respawn Point atanmadı!"
            );
        }
    }


    private void FixedUpdate()
    {
        // FallZone herhangi bir nedenle karakteri
        // yakalayamazsa ikinci güvenlik sistemi.
        if (!isRespawning &&
            transform.position.y <= fallYThreshold)
        {
            Respawn();
        }
    }


    public void SetRespawnPoint(
        Transform newRespawnPoint)
    {
        if (newRespawnPoint == null)
        {
            return;
        }


        currentRespawnPoint =
            newRespawnPoint;
    }


    public void Respawn()
    {
        if (isRespawning)
        {
            return;
        }


        if (currentRespawnPoint == null)
        {
            return;
        }


        isRespawning =
            true;


        // Düşmeden kalan bütün fizik hareketini temizle.
        rb.linearVelocity =
            Vector3.zero;


        rb.angularVelocity =
            Vector3.zero;


        Vector3 respawnPosition =
            currentRespawnPoint.position +
            Vector3.up *
            respawnVerticalOffset;


        rb.position =
            respawnPosition;


        rb.rotation =
            currentRespawnPoint.rotation;


        rb.linearVelocity =
            Vector3.zero;


        rb.angularVelocity =
            Vector3.zero;


        Physics.SyncTransforms();


        // AI ise State Machine'i de
        // bulunduğu bölgeye geri hazırla.
        if (aiController != null)
        {
            aiController.HandleRespawn();
        }


        Debug.Log(
            gameObject.name +
            " güvenli noktaya Respawn oldu."
        );


        isRespawning =
            false;
    }
}