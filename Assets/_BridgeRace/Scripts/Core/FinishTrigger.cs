using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private HashSet<CharacterBase>
        finishedCharacters =
            new HashSet<CharacterBase>();


    private void OnTriggerEnter(
        Collider other)
    {
        // Collider karakterin kendisinde
        // veya child objesinde olabilir.
        CharacterBase character =
            other.GetComponentInParent<CharacterBase>();


        if (character == null)
        {
            return;
        }


        // Aynı karakter ikinci kez
        // Finish sayılmasın.
        if (finishedCharacters.Contains(
                character))
        {
            return;
        }


        finishedCharacters.Add(
            character
        );


        Debug.Log(
            character.gameObject.name +
            " FinishTrigger'a girdi!"
        );


        EventManager.CharacterFinished(
            character
        );
    }
}