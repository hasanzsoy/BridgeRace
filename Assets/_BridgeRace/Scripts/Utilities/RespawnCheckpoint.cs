using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour
{
    [Header("Respawn Point")]
    [SerializeField] private Transform respawnPoint;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterRespawn>(
                out CharacterRespawn characterRespawn))
        {
            characterRespawn.SetRespawnPoint(
                respawnPoint
            );
        }
    }
}