using UnityEngine;

public static class SaveManager
{
    private const string DifficultyKey =
        "SelectedDifficulty";

    private const string GoldKey =
        "PlayerGold";


    // ==========================================
    // DIFFICULTY
    // ==========================================

    public static void SaveDifficulty(
        AIDifficulty difficulty)
    {
        PlayerPrefs.SetInt(
            DifficultyKey,
            (int)difficulty
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Difficulty kaydedildi: " +
            difficulty
        );
    }


    public static AIDifficulty LoadDifficulty()
    {
        int savedDifficulty =
            PlayerPrefs.GetInt(
                DifficultyKey,
                (int)AIDifficulty.Normal
            );

        return
            (AIDifficulty)savedDifficulty;
    }


    // ==========================================
    // GOLD
    // ==========================================

    public static int LoadGold()
    {
        return PlayerPrefs.GetInt(
            GoldKey,
            0
        );
    }


    public static void SaveGold(
        int gold)
    {
        gold =
            Mathf.Max(
                gold,
                0
            );

        PlayerPrefs.SetInt(
            GoldKey,
            gold
        );

        PlayerPrefs.Save();
    }


    public static int AddGold(
        int amount)
    {
        int currentGold =
            LoadGold();

        currentGold +=
            amount;

        SaveGold(
            currentGold
        );

        return currentGold;
    }
}