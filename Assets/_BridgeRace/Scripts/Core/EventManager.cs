using System;

public static class EventManager
{
    public static event Action<CharacterBase, int>
        OnBrickCollected;

    public static event Action<CharacterBase, int>
        OnBrickSpent;

    public static event Action<CharacterBase, int>
        OnBrickDropped;

    public static event Action<CharacterBase>
        OnCharacterKnockback;

    public static event Action<CharacterBase>
        OnCharacterFinished;

    public static event Action
        OnRaceStarted;

    public static event Action
        OnRaceFinished;


    public static void BrickCollected(
        CharacterBase character,
        int stackCount)
    {
        OnBrickCollected?.Invoke(
            character,
            stackCount
        );
    }


    public static void BrickSpent(
        CharacterBase character,
        int stackCount)
    {
        OnBrickSpent?.Invoke(
            character,
            stackCount
        );
    }


    public static void BrickDropped(
        CharacterBase character,
        int stackCount)
    {
        OnBrickDropped?.Invoke(
            character,
            stackCount
        );
    }


    public static void CharacterKnockback(
        CharacterBase character)
    {
        OnCharacterKnockback?.Invoke(
            character
        );
    }


    public static void CharacterFinished(
        CharacterBase character)
    {
        OnCharacterFinished?.Invoke(
            character
        );
    }


    public static void RaceStarted()
    {
        OnRaceStarted?.Invoke();
    }


    public static void RaceFinished()
    {
        OnRaceFinished?.Invoke();
    }
}