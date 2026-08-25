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
                1,
                ranking[0]
            );
        }


        if (ranking.Length > 1)
        {
            SetRow(
                secondPlaceText,
                2,
                ranking[1]
            );
        }


        if (ranking.Length > 2)
        {
            SetRow(
                thirdPlaceText,
                3,
                ranking[2]
            );
        }


        if (ranking.Length > 3)
        {
            SetRow(
                fourthPlaceText,
                4,
                ranking[3]
            );
        }
    }


    private void SetRow(
        TMP_Text text,
        int place,
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


        int brickCount =
            GetBrickCount(
                character
            );


        text.text =
            place +
            ". " +
            characterName +
            "     " +
            brickCount;
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