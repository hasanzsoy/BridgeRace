public class NormalDifficultyStrategy :
    IAIDifficultyStrategy
{
    private readonly float moveSpeed;
    private readonly float searchInterval;

    private readonly int brickGoal;


    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
    }


    public float SearchInterval
    {
        get
        {
            return searchInterval;
        }
    }


    public AIBrickSearchMode SearchMode
    {
        get
        {
            return AIBrickSearchMode.Nearest;
        }
    }


    public NormalDifficultyStrategy(
        float moveSpeed,
        float searchInterval,
        int brickGoal)
    {
        this.moveSpeed =
            moveSpeed;

        this.searchInterval =
            searchInterval;

        this.brickGoal =
            brickGoal;
    }


    public int GetBrickGoal()
    {
        return brickGoal;
    }
}