public interface IBuildable
{
    bool NeedsBuild(TeamColor builderColor);

    void BuildStep(TeamColor builderColor);
}