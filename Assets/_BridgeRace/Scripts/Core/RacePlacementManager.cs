using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RacePlacementManager : MonoBehaviour
{
    [Header("Podium Points")]
    [SerializeField] private Transform firstPlacePoint;
    [SerializeField] private Transform secondPlacePoint;
    [SerializeField] private Transform thirdPlacePoint;
    [SerializeField] private Transform fourthPlacePoint;

    [Header("Race Settings")]
    [SerializeField] private int totalRacers = 4;

    [Header("Podium Animation")]
    [SerializeField] private float podiumMoveDuration = 0.5f;

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


        // Aynı karakter iki defa
        // sıralamaya giremesin.
        if (finishedCharacters.Contains(character))
        {
            return;
        }


        finishedCharacters.Add(character);


        int place =
            finishedCharacters.Count;


        Transform placePoint =
            GetPlacePoint(place);


        if (placePoint == null)
        {
            return;
        }


        SendCharacterToPodium(
            character,
            placePoint
        );


        EventManager.CharacterPlaced(
            character,
            place
        );


        Debug.Log(
            character.gameObject.name +
            " yarışı " +
            place +
            ". sırada bitirdi."
        );


        // 4 yarışçı da bitirdiyse
        // yarış tamamen bitmiş olur.
        if (finishedCharacters.Count >=
            totalRacers)
        {
            EventManager.RaceFinished();
        }
    }


    private void SendCharacterToPodium(
        CharacterBase character,
        Transform placePoint)
    {
        // Karakter artık hareket edemesin.
        character.SetMovementEnabled(false);


        // Varsa eski DOTween hareketlerini temizle.
        character.transform.DOKill();


        // Fizik hareketini durdur.
        if (character.TryGetComponent<Rigidbody>(
                out Rigidbody rb))
        {
            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;

            rb.isKinematic =
                true;
        }


        // Podiumda karakterler birbirini
        // itmesin.
        if (character.TryGetComponent<Collider>(
                out Collider characterCollider))
        {
            characterCollider.enabled =
                false;
        }


        // Direkt ışınlamak yerine
        // kısa bir geçiş animasyonu.
        character.transform.DOMove(
            placePoint.position,
            podiumMoveDuration
        )
        .SetEase(Ease.OutQuad);


        character.transform.DORotateQuaternion(
            placePoint.rotation,
            podiumMoveDuration
        );
    }


    private Transform GetPlacePoint(
        int place)
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