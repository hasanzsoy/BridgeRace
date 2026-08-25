using System;
using UnityEngine;

public class LiveRaceRankingManager : MonoBehaviour
{
    [Header("Update Settings")]
    [SerializeField]
    private float updateInterval = 0.15f;


    private CharacterBase[] racers;

    private float nextUpdateTime;

    private bool raceFinished;


    private void Start()
    {
        racers =
            FindObjectsByType<CharacterBase>(
                FindObjectsSortMode.None
            );


        if (racers == null ||
            racers.Length == 0)
        {
            Debug.LogError(
                "LiveRaceRankingManager yarışçı bulamadı!"
            );

            return;
        }


        // Oyun başladığı anda ilk sıralamayı gönder.
        UpdateRanking();
    }


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


    private void Update()
    {
        if (raceFinished)
        {
            return;
        }


        if (Time.time <
            nextUpdateTime)
        {
            return;
        }


        nextUpdateTime =
            Time.time +
            updateInterval;


        UpdateRanking();
    }


    private void UpdateRanking()
    {
        if (racers == null ||
            racers.Length == 0)
        {
            return;
        }


        CharacterBase[] ranking =
            new CharacterBase[
                racers.Length
            ];


        Array.Copy(
            racers,
            ranking,
            racers.Length
        );


        Array.Sort(
            ranking,
            CompareRacers
        );


        // Sıralama EventManager üzerinden UI'a gider.
        // Her güncellemede gönderiyoruz çünkü
        // brick sayısı sıra değişmese bile değişebilir.
        EventManager.LiveRankingChanged(
            ranking
        );
    }


    private int CompareRacers(
        CharacterBase first,
        CharacterBase second)
    {
        if (first == null &&
            second == null)
        {
            return 0;
        }


        if (first == null)
        {
            return 1;
        }


        if (second == null)
        {
            return -1;
        }


        // =========================================
        // 1. ÖNCELİK:
        // Yarışta ne kadar ileride?
        // =========================================

        float firstProgress =
            first.transform.position.z;

        float secondProgress =
            second.transform.position.z;


        int progressCompare =
            secondProgress.CompareTo(
                firstProgress
            );


        if (progressCompare != 0)
        {
            return progressCompare;
        }


        // =========================================
        // 2. ÖNCELİK:
        // İkisi aynı yerdeyse brick sayısı fazla
        // olan önde gösterilsin.
        // =========================================

        int firstBrickCount =
            GetBrickCount(
                first
            );

        int secondBrickCount =
            GetBrickCount(
                second
            );


        return secondBrickCount.CompareTo(
            firstBrickCount
        );
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


    private void OnCharacterFinished(
        CharacterBase character)
    {
        if (raceFinished)
        {
            return;
        }


        // Son kez güncelle.
        UpdateRanking();


        // Bizim oyunda ilk finish ile sonuç ekranı
        // başladığı için canlı leaderboard'u dondur.
        raceFinished =
            true;
    }
}