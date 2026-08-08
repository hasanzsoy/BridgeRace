using UnityEngine;

public class FallZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterRespawn>(
                out CharacterRespawn characterRespawn))
        {
            characterRespawn.Respawn();
        }
    }
}