using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private Transform startingRespawnPoint;

    private Transform currentRespawnPoint;
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        currentRespawnPoint = startingRespawnPoint;

        if (startingRespawnPoint == null)
        {
            Debug.LogError(
                gameObject.name +
                " için Starting Respawn Point atanmadı!"
            );
        }
    }


    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        if (newRespawnPoint == null)
        {
            return;
        }

        currentRespawnPoint = newRespawnPoint;
    }


    public void Respawn()
    {
        if (currentRespawnPoint == null)
        {
            return;
        }

        // Düşerken sahip olduğu hızları temizle.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Son güvenli noktaya götür.
        rb.position = currentRespawnPoint.position;
        rb.rotation = currentRespawnPoint.rotation;
    }
}