using GitVersion.BranchingStrategies.GitHubFlow;

namespace GitVersion.BranchingStrategies
{
    public class GitHubFlowStrategy : IBranchingStrategy
    {
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;

        public GitHubFlowStrategy(ILastTaggedReleaseFinder lastTaggedReleaseFinder)
        {
            _lastTaggedReleaseFinder = lastTaggedReleaseFinder;
        }

        public void ValidateGitRepository()
        {
            
        }

        public ISemanticVersionCalculator GetNextSemverCalculator(GitVersionContext context)
        {
            var nextVersionTxtFileFinder = new NextVersionTxtFileFinder(context.RepositoryRoot);
            return new GitHubFlowSemamticVersionCalculator(nextVersionTxtFileFinder, _lastTaggedReleaseFinder);
        }
    }
}