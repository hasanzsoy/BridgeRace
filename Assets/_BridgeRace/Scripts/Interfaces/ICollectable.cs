public interface ICollectable
{
    TeamColor CollectableColor { get; }

    void Collect(CharacterBase collector);
}