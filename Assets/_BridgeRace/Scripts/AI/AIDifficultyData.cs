using UnityEngine;

[CreateAssetMenu(fileName = "AI_Difficulty_Data",menuName = "Bridge Race/AI Difficulty Data")]
public class AIDifficultyData : ScriptableObject
{
    [Header("Difficulty")]
    public AIDifficulty difficulty;

    [Header("Movement")]
    [Min(0f)]
    public float moveSpeed = 4f;

    [Header("Brick Search")]
    [Min(0.01f)]
    public float searchInterval = 0.5f;

    [Header("Brick Goal")]
    [Min(0)]
    public int minBrickGoal = 5;

    [Min(0)]
    public int maxBrickGoal = 12;

    [Header("Brick Targeting")]
    [Min(0f)]
    public float clusterRadius = 4f;

    [Header("Opponent Behaviour")]
    [Min(0f)]
    public float avoidanceRadius = 2.5f;

    [Min(0f)]
    public float avoidanceStrength = 2f;

    [Min(0f)]
    public float attackRadius = 5f;

    [Min(0)]
    public int brickAdvantage = 3;
}