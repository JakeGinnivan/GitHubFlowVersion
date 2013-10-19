namespace GitHubFlowVersion
{
    public interface INextSemverCalculator
    {
        SemanticVersion NextVersion();
    }
}