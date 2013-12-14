using GitVersion.BranchingStrategies.GitHubFlow;

namespace GitVersion.BranchingStrategies
{
    public class GitFlowStrategy : IBranchingStrategy
    {
        public void ValidateGitRepository()
        {

        }

        public ISemanticVersionCalculator GetNextSemverCalculator(GitVersionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}