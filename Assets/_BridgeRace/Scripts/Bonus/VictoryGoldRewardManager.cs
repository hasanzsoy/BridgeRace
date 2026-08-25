using UnityEngine;

public class VictoryGoldRewardManager : MonoBehaviour
{
    [Header("Gold Reward Settings")]
    [SerializeField]
    private int baseGoldReward = 100;

    [SerializeField]
    private int goldPerBrick = 2;


    private bool rewardGiven;


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


        // Sadece gerçek Player için ödül hesapla.
        if (!(character is PlayerController))
        {
            return;
        }


        int remainingBrickCount = 0;


        if (character.TryGetComponent<CharacterStack>(
                out CharacterStack characterStack))
        {
            remainingBrickCount =
                characterStack.BrickCount;
        }


        // =========================================
        // GOLD FORMÜLÜ
        //
        // 100 + (Kalan Brick × 2)
        // =========================================

        int brickBonus =
            remainingBrickCount *
            goldPerBrick;


        int rewardGold =
            baseGoldReward +
            brickBonus;


        int oldGold =
            SaveManager.LoadGold();


        int newTotalGold =
            SaveManager.AddGold(
                rewardGold
            );


        rewardGiven =
            true;


        // Victory UI'ya EventManager üzerinden haber ver.
        EventManager.VictoryGoldReward(
            oldGold,
            rewardGold,
            newTotalGold
        );


        // İleride shop vb. sistemler için
        // mevcut Gold eventini de güncel tut.
        EventManager.GoldChanged(
            newTotalGold
        );


        Debug.Log(
            "VICTORY GOLD | " +
            remainingBrickCount +
            " Brick | 100 + (" +
            remainingBrickCount +
            " x " +
            goldPerBrick +
            ") = " +
            rewardGold +
            " Gold | Toplam Gold: " +
            newTotalGold
        );
    }
}