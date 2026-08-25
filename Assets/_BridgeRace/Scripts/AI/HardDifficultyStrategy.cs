public class HardDifficultyStrategy :
    IAIDifficultyStrategy
{
    private readonly float moveSpeed;
    private readonly float searchInterval;

    private readonly int bricksNeededForBridge;


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
            return AIBrickSearchMode.Cluster;
        }
    }


    public HardDifficultyStrategy(
        float moveSpeed,
        float searchInterval,
        int bricksNeededForBridge)
    {
        this.moveSpeed =
            moveSpeed;

        this.searchInterval =
            searchInterval;

        this.bricksNeededForBridge =
            bricksNeededForBridge;
    }


    public int GetBrickGoal()
    {
        return bricksNeededForBridge;
    }
}