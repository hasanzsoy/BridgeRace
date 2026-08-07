using UnityEngine;

public class CharacterCollectDetector : MonoBehaviour
{
    private CharacterBase character;


    private void Awake()
    {
        character = GetComponent<CharacterBase>();

        if (character == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterBase bulunamadı!"
            );
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (character == null)
        {
            return;
        }


        if (other.TryGetComponent<ICollectable>(
                out ICollectable collectable))
        {
            collectable.Collect(character);
        }
    }
}