namespace GitVersion.BranchingStrategies.GitHubFlow
{
    public interface INextVersionTxtFileFinder
    {
        SemanticVersion GetNextVersion(SemanticVersion taggedVersion);
    }
}