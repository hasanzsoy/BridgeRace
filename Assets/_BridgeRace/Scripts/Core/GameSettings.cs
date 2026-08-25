public static class GameSettings
{
    public static AIDifficulty SelectedDifficulty
    {
        get;
        set;
    } =
        SaveManager.LoadDifficulty();
}