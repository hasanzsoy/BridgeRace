public interface IBrickModifierTarget
{
    int CurrentBrickCount { get; }

    void AddBricks(int amount);

    void RemoveBricks(int amount);
}