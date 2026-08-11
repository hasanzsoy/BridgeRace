using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private HashSet<CharacterBase> finishedCharacters =
        new HashSet<CharacterBase>();


    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<CharacterBase>(
                out CharacterBase character))
        {
            return;
        }


        // Aynı karakter ikinci kez finish olarak
        // sayılmasın.
        if (finishedCharacters.Contains(character))
        {
            return;
        }


        finishedCharacters.Add(character);


        EventManager.CharacterFinished(
            character
        );
    }
}