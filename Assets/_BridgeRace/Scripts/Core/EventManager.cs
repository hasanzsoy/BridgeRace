using System;

public static class EventManager
{
    // Brick toplandığında
    public static event Action<CharacterBase, int>
        OnBrickCollected;

    // Köprü yaparken Brick harcandığında
    public static event Action<CharacterBase, int>
        OnBrickSpent;

    // Knockback sonrası Brickler düştüğünde
    public static event Action<CharacterBase, int>
        OnBrickDropped;

    // Karakter knockback aldığında
    public static event Action<CharacterBase>
        OnCharacterKnockback;

    // Karakter FinishTrigger'a girdiğinde
    public static event Action<CharacterBase>
        OnCharacterFinished;

    // Karakterin yarış sırası belli olduğunda
    public static event Action<CharacterBase, int>
        OnCharacterPlaced;

    // Yarış başladığında
    public static event Action
        OnRaceStarted;

    // Bütün yarışçılar bitirdiğinde
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


    public static void CharacterPlaced(
        CharacterBase character,
        int place)
    {
        OnCharacterPlaced?.Invoke(
            character,
            place
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