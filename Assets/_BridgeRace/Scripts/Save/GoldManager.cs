using UnityEngine;

public class GoldManager : MonoBehaviour
{
    [Header("Race Rewards")]
    [SerializeField] private int firstPlaceReward = 100;
    [SerializeField] private int secondPlaceReward = 60;
    [SerializeField] private int thirdPlaceReward = 40;
    [SerializeField] private int fourthPlaceReward = 20;


    private bool rewardGiven;


    private void Start()
    {
        int savedGold =
            SaveManager.LoadGold();

        EventManager.GoldChanged(
            savedGold
        );

        Debug.Log(
            "Kayıtlı Gold: " +
            savedGold
        );
    }


    private void OnEnable()
    {
        EventManager.OnCharacterPlaced +=
            OnCharacterPlaced;
    }


    private void OnDisable()
    {
        EventManager.OnCharacterPlaced -=
            OnCharacterPlaced;
    }


    private void OnCharacterPlaced(
        CharacterBase character,
        int place)
    {
        if (rewardGiven)
        {
            return;
        }


        if (character == null)
        {
            return;
        }


        // Sadece gerçek Player ödül alır.
        if (!(character is PlayerController))
        {
            return;
        }


        int reward =
            GetReward(
                place
            );


        int newGold =
            SaveManager.AddGold(
                reward
            );


        rewardGiven =
            true;


        EventManager.GoldChanged(
            newGold
        );


        EventManager.GoldRewardEarned(
            reward
        );


        Debug.Log(
            "Player " +
            place +
            ". oldu. +" +
            reward +
            " Gold | Toplam: " +
            newGold
        );
    }


    private int GetReward(
        int place)
    {
        switch (place)
        {
            case 1:
                return firstPlaceReward;

            case 2:
                return secondPlaceReward;

            case 3:
                return thirdPlaceReward;

            case 4:
                return fourthPlaceReward;
        }


        return 0;
    }
}