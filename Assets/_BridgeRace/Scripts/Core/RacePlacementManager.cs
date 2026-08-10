using System.Collections.Generic;
using UnityEngine;

public class RacePlacementManager : MonoBehaviour
{
    [Header("Podium Points")]
    [SerializeField] private Transform firstPlacePoint;
    [SerializeField] private Transform secondPlacePoint;
    [SerializeField] private Transform thirdPlacePoint;
    [SerializeField] private Transform fourthPlacePoint;


    private List<CharacterBase> finishedCharacters =
        new List<CharacterBase>();


    private void OnEnable()
    {
        EventManager.OnCharacterFinished +=
            OnCharacterFinished;
    }


    private void OnDisable()
    {
        EventManager.OnCharacterFinished -=
            OnCharacterFinished;
    }


    private void OnCharacterFinished(
        CharacterBase character)
    {
        if (character == null)
        {
            return;
        }


        if (finishedCharacters.Contains(character))
        {
            return;
        }


        finishedCharacters.Add(character);


        int place =
            finishedCharacters.Count;


        Transform targetPoint =
            GetPlacePoint(place);


        if (targetPoint == null)
        {
            return;
        }


        character.SetMovementEnabled(false);


        if (character.TryGetComponent<Rigidbody>(
                out Rigidbody rb))
        {
            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;

            rb.position =
                targetPoint.position;

            rb.rotation =
                targetPoint.rotation;
        }
        else
        {
            character.transform.position =
                targetPoint.position;

            character.transform.rotation =
                targetPoint.rotation;
        }


        Debug.Log(
            character.gameObject.name +
            " yarışı " +
            place +
            ". sırada bitirdi."
        );
    }


    private Transform GetPlacePoint(int place)
    {
        switch (place)
        {
            case 1:
                return firstPlacePoint;

            case 2:
                return secondPlacePoint;

            case 3:
                return thirdPlacePoint;

            case 4:
                return fourthPlacePoint;
        }


        return null;
    }
}