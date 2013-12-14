namespace GitVersion.BranchingStrategies.GitHubFlow
{
    public interface ISemanticVersionCalculator
    {
        SemanticVersion NextVersion();
    }
}