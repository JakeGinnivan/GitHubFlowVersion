using GitVersion.BranchingStrategies.GitHubFlow;

namespace GitVersion.BranchingStrategies
{
    public interface IBranchingStrategy
    {
        void ValidateGitRepository();
        ISemanticVersionCalculator GetNextSemverCalculator(GitVersionContext context);
    }
}