public interface IAIDifficultyStrategy
{
    float MoveSpeed
    {
        get;
    }


    float SearchInterval
    {
        get;
    }


    AIBrickSearchMode SearchMode
    {
        get;
    }


    int GetBrickGoal();
}