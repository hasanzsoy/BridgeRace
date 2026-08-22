using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class RacePlacementManager : MonoBehaviour
{
    [Header("Podium Points")]
    [SerializeField] private Transform firstPlacePoint;
    [SerializeField] private Transform secondPlacePoint;
    [SerializeField] private Transform thirdPlacePoint;

    [Header("Ranking Settings")]
    [SerializeField] private Transform finishReference;

    [Header("Podium Animation")]
    [SerializeField] private float podiumMoveDuration = 0.6f;

    private bool raceResolved;


    private void OnEnable()
    {
        EventManager.OnCharacterFinished += OnCharacterFinished;
    }


    private void OnDisable()
    {
        EventManager.OnCharacterFinished -= OnCharacterFinished;
    }


    private void OnCharacterFinished(CharacterBase winner)
    {
        if (winner == null)
        {
            return;
        }

        // Yarış bir kere sonuçlandırılır.
        if (raceResolved)
        {
            return;
        }

        raceResolved = true;

        Debug.Log(
            winner.gameObject.name +
            " yarışı ilk bitirdi!"
        );


        // Sahnede bulunan bütün aktif yarışçıları al.
        CharacterBase[] foundRacers =
            FindObjectsByType<CharacterBase>(
                FindObjectsSortMode.None
            );


        List<CharacterBase> remainingRacers =
            new List<CharacterBase>();


        for (int i = 0; i < foundRacers.Length; i++)
        {
            CharacterBase racer =
                foundRacers[i];

            if (racer == null)
            {
                continue;
            }

            // Winner zaten kesin 1.
            if (racer == winner)
            {
                continue;
            }

            remainingRacers.Add(racer);
        }


        // =========================================
        // Kalanları Brick sayılarına göre sırala.
        // Fazla Brick = daha iyi sıra.
        // =========================================

        remainingRacers.Sort(
            CompareRemainingRacers
        );


        List<CharacterBase> finalOrder =
            new List<CharacterBase>();


        // İlk bitiren kesin birinci.
        finalOrder.Add(winner);


        // Geri kalanlar Brick sayısına göre.
        finalOrder.AddRange(
            remainingRacers
        );


        // Yarış bittiği anda herkes durur.
        for (int i = 0;
             i < finalOrder.Count;
             i++)
        {
            FreezeCharacter(
                finalOrder[i]
            );
        }


        // =========================================
        // Sıralamayı uygula.
        // =========================================

        for (int i = 0;
             i < finalOrder.Count;
             i++)
        {
            CharacterBase racer =
                finalOrder[i];

            int place = i + 1;


            if (place <= 3)
            {
                Transform placePoint =
                    GetPlacePoint(place);


                if (placePoint != null)
                {
                    SendCharacterToPodium(
                        racer,
                        placePoint
                    );
                }
                else
                {
                    Debug.LogError(
                        place +
                        ". sıra için Place Point atanmadı!"
                    );
                }
            }


            // AI ve diğer sistemlere
            // sonucunu bildir.
            EventManager.CharacterPlaced(
                racer,
                place
            );


            Debug.Log(
                racer.gameObject.name +
                " → " +
                place +
                ". sıra | Brick: " +
                GetBrickCount(racer)
            );
        }


        // Artık diğerlerinin finish'e
        // gelmesini beklemiyoruz.
        EventManager.RaceFinished();
    }


    private int CompareRemainingRacers(
        CharacterBase a,
        CharacterBase b)
    {
        int aBrick =
            GetBrickCount(a);

        int bBrick =
            GetBrickCount(b);


        // Büyük Brick sayısı önce gelsin.
        int brickComparison =
            bBrick.CompareTo(aBrick);


        if (brickComparison != 0)
        {
            return brickComparison;
        }


        // Brick sayıları eşitse,
        // Finish'e daha yakın olan öne geçsin.
        if (finishReference != null)
        {
            float aDistance =
                (
                    a.transform.position -
                    finishReference.position
                ).sqrMagnitude;


            float bDistance =
                (
                    b.transform.position -
                    finishReference.position
                ).sqrMagnitude;


            return aDistance.CompareTo(
                bDistance
            );
        }


        return 0;
    }


    private int GetBrickCount(
        CharacterBase character)
    {
        if (character == null)
        {
            return 0;
        }


        if (character.TryGetComponent<CharacterStack>(
                out CharacterStack stack))
        {
            return stack.BrickCount;
        }


        return 0;
    }


    private void FreezeCharacter(
        CharacterBase character)
    {
        if (character == null)
        {
            return;
        }


        // CharacterBase hareketini durdur.
        character.SetMovementEnabled(
            false
        );


        // Eski Tween varsa temizle.
        character.transform.DOKill();


        // AI ise NavMesh hareketini de durdur.
        if (character.TryGetComponent<NavMeshAgent>(
                out NavMeshAgent agent))
        {
            if (agent.enabled &&
                agent.isOnNavMesh)
            {
                agent.ResetPath();
            }

            agent.enabled = false;
        }


        // Rigidbody tamamen dursun.
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


        // Yarış bittikten sonra
        // karakterler birbirini itmesin.
        if (character.TryGetComponent<Collider>(
                out Collider characterCollider))
        {
            characterCollider.enabled =
                false;
        }
    }


    private void SendCharacterToPodium(
        CharacterBase character,
        Transform placePoint)
    {
        character.transform.DOMove(
                placePoint.position,
                podiumMoveDuration
            )
            .SetEase(
                Ease.OutQuad
            );


        character.transform
            .DORotateQuaternion(
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