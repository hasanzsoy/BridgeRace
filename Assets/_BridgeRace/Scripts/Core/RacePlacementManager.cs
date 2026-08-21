using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RacePlacementManager : MonoBehaviour
{
    [Header("Podium Points")]
    [SerializeField] private Transform firstPlacePoint;
    [SerializeField] private Transform secondPlacePoint;
    [SerializeField] private Transform thirdPlacePoint;


    [Header("Race Settings")]
    [SerializeField] private int totalRacers = 4;


    [Header("Podium Animation")]
    [SerializeField] private float podiumMoveDuration = 0.6f;


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


        // Aynı karakter ikinci kez
        // sıralamaya giremez.
        if (finishedCharacters.Contains(
                character))
        {
            return;
        }


        finishedCharacters.Add(
            character
        );


        int place =
            finishedCharacters.Count;


        Debug.Log(
            character.gameObject.name +
            " FINISH'e ulaştı. Sıra: " +
            place
        );


        // ==========================================
        // İlk 3 karakter podiuma gider.
        // ==========================================

        if (place <= 3)
        {
            Transform placePoint =
                GetPlacePoint(place);


            if (placePoint != null)
            {
                SendCharacterToPodium(
                    character,
                    placePoint
                );
            }
            else
            {
                Debug.LogError(
                    place +
                    ". sıra için Place Point atanmadı!"
                );


                StopCharacterAtFinish(
                    character
                );
            }
        }

        // ==========================================
        // 4. karakter podiuma çıkmaz.
        // ==========================================

        else
        {
            StopCharacterAtFinish(
                character
            );
        }


        // AIController / UI gibi sistemlere
        // karakterin sırasını bildir.
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


        // Bütün yarışçılar bitirdi.
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
        // Artık normal hareket etmesin.
        character.SetMovementEnabled(
            false
        );


        // Eski tween varsa temizle.
        character.transform.DOKill();


        // Fizik sistemini durdur.
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


        // Podium üzerinde karakterlerin
        // birbirini itmesini engelle.
        if (character.TryGetComponent<Collider>(
                out Collider characterCollider))
        {
            characterCollider.enabled =
                false;
        }


        // ==========================================
        // Podium hareketi
        // ==========================================

        character.transform.DOMove(
                placePoint.position,
                podiumMoveDuration
            )
            .SetEase(
                Ease.OutQuad
            );


        character.transform.DORotateQuaternion(
                placePoint.rotation,
                podiumMoveDuration
            )
            .SetEase(
                Ease.OutQuad
            );


        Debug.Log(
            character.gameObject.name +
            " → " +
            placePoint.gameObject.name
        );
    }


    private void StopCharacterAtFinish(
        CharacterBase character)
    {
        character.SetMovementEnabled(
            false
        );


        character.transform.DOKill();


        if (character.TryGetComponent<Rigidbody>(
                out Rigidbody rb))
        {
            rb.linearVelocity =
                Vector3.zero;


            rb.angularVelocity =
                Vector3.zero;
        }


        Debug.Log(
            character.gameObject.name +
            " 4. oldu. Podiuma çıkmayacak."
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
        }


        return null;
    }
}