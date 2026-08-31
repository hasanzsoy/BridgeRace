using System;

public static class EventManager
{
    public static event Action<CharacterBase, int>OnBrickCollected;
    public static event Action<CharacterBase, int>OnBrickSpent;
    public static event Action<CharacterBase, int> OnBrickDropped;
    public static event Action<CharacterBase> OnCharacterKnockback;
    public static event Action<CharacterBase> OnCharacterFinished;
    public static event Action<CharacterBase, int> OnCharacterPlaced;
    public static event Action OnRaceStarted;
    public static event Action OnRaceFinished;
    public static event Action<int, int>OnComboChanged;
    public static event Action<int>OnComboCompleted;
    public static event Action<CharacterBase[]>OnLiveRankingChanged;

    public static event Action<int>OnGoldChanged;

    public static event Action<int>OnGoldRewardEarned;

    public static event Action<int, int, int>OnVictoryGoldReward;

    public static event Action<bool>OnGameSoundChanged;

    public static event Action<bool> OnBrickSoundChanged;

    public static event Action<bool>OnBridgeSoundChanged;

    public static void BrickCollected(CharacterBase character,int stackCount)
    {
        OnBrickCollected?.Invoke(character,stackCount);
    }

    public static void BrickSpent(CharacterBase character,int stackCount)
    {
        OnBrickSpent?.Invoke(character,stackCount);
    }

    public static void BrickDropped(CharacterBase character,int stackCount)
    {
        OnBrickDropped?.Invoke(character,stackCount);
    }

    public static void CharacterKnockback(CharacterBase character)
    {
        OnCharacterKnockback?.Invoke(character);
    }

    public static void CharacterFinished(CharacterBase character)
    {
        OnCharacterFinished?.Invoke(character);
    }


    public static void CharacterPlaced(CharacterBase character,int place)
    {
        OnCharacterPlaced?.Invoke(character,place);
    }

    public static void RaceStarted()
    {
        OnRaceStarted?.Invoke();
    }

    public static void RaceFinished()
    {
        OnRaceFinished?.Invoke();
    }


    public static void ComboChanged(int currentCombo,int requiredCombo)
    {
        OnComboChanged?.Invoke(currentCombo,requiredCombo);
    }

    public static void ComboCompleted(int bonusBrickAmount)
    {
        OnComboCompleted?.Invoke(bonusBrickAmount);
    }

    public static void LiveRankingChanged(CharacterBase[] ranking)
    {
        OnLiveRankingChanged?.Invoke(ranking);
    }
    public static void GoldChanged(int totalGold)
    {
        OnGoldChanged?.Invoke(totalGold);
    }

    public static void GoldRewardEarned(int reward)
    {
        OnGoldRewardEarned?.Invoke(reward);
    }

    public static void VictoryGoldReward(int oldGold,int rewardGold,int newTotalGold)
    {
        OnVictoryGoldReward?.Invoke(oldGold,rewardGold,newTotalGold);
    }

    public static void GameSoundChanged(bool enabled)
    {
        OnGameSoundChanged?.Invoke(enabled);
    }

    public static void BrickSoundChanged(bool enabled)
    {
        OnBrickSoundChanged?.Invoke(enabled);
    }
    public static void BridgeSoundChanged(bool enabled)
    {
        OnBridgeSoundChanged?.Invoke(enabled);
    }
}