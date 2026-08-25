using UnityEngine;

public class EasyDifficultyStrategy :
    IAIDifficultyStrategy
{
    private readonly float moveSpeed;
    private readonly float searchInterval;

    private readonly int minBricks;
    private readonly int maxBricks;


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
            return AIBrickSearchMode.Random;
        }
    }

    public AIOpponentMode OpponentMode
    {
        get
        {
            return AIOpponentMode.Avoid;
        }
    }

    public EasyDifficultyStrategy(
        float moveSpeed,
        float searchInterval,
        int minBricks,
        int maxBricks)
    {
        this.moveSpeed =
            moveSpeed;

        this.searchInterval =
            searchInterval;

        this.minBricks =
            minBricks;

        this.maxBricks =
            maxBricks;
    }


    public int GetBrickGoal()
    {
        return Random.Range(
            minBricks,
            maxBricks + 1
        );
    }
}