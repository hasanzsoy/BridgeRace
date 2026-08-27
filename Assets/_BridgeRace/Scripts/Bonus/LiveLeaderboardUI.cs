using TMPro;
using UnityEngine;

public class LiveLeaderboardUI : MonoBehaviour
{
    [Header("Ranking Texts")]

    [SerializeField]
    private TMP_Text firstPlaceText;

    [SerializeField]
    private TMP_Text secondPlaceText;

    [SerializeField]
    private TMP_Text thirdPlaceText;

    [SerializeField]
    private TMP_Text fourthPlaceText;


    [Header("AI Names")]

    [SerializeField]
    private string redName = "LIAM";

    [SerializeField]
    private string greenName = "EMMA";

    [SerializeField]
    private string yellowName = "NOAH";


    // =====================================================
    // EVENTS
    // =====================================================

    private void OnEnable()
    {
        EventManager.OnLiveRankingChanged +=
            UpdateLeaderboard;
    }


    private void OnDisable()
    {
        EventManager.OnLiveRankingChanged -=
            UpdateLeaderboard;
    }


    // =====================================================
    // LEADERBOARD
    // =====================================================

    private void UpdateLeaderboard(
        CharacterBase[] ranking)
    {
        if (ranking == null)
        {
            return;
        }


        if (ranking.Length > 0)
        {
            SetRow(
                firstPlaceText,
                ranking[0]
            );
        }


        if (ranking.Length > 1)
        {
            SetRow(
                secondPlaceText,
                ranking[1]
            );
        }


        if (ranking.Length > 2)
        {
            SetRow(
                thirdPlaceText,
                ranking[2]
            );
        }


        if (ranking.Length > 3)
        {
            SetRow(
                fourthPlaceText,
                ranking[3]
            );
        }
    }


    // =====================================================
    // ROW
    // =====================================================

    private void SetRow(
        TMP_Text text,
        CharacterBase character)
    {
        if (text == null ||
            character == null)
        {
            return;
        }


        string characterName =
            GetCharacterName(
                character
            );


        // Sadece karakter ismini gösteriyoruz.
        // Brick sayısı ve sıra numarası gösterilmiyor.
        text.text =
            characterName;
    }


    // =====================================================
    // CHARACTER NAME
    // =====================================================

    private string GetCharacterName(
        CharacterBase character)
    {
        if (character is PlayerController)
        {
            return "YOU";
        }


        switch (
            character.CharacterTeamColor)
        {
            case TeamColor.Red:

                return redName;


            case TeamColor.Green:

                return greenName;


            case TeamColor.Yellow:

                return yellowName;
        }


        return character.gameObject.name;
    }
}