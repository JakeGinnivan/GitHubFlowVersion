namespace GitVersion
{
    public interface INextSemverCalculator
    {
        SemanticVersion NextVersion();
    }
}