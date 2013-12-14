using System;
using LibGit2Sharp;

namespace GitHubFlowVersion.BuildServers
{
    public class TeamCity : IBuildServer
    {
        private readonly ILog _log;
        private readonly IGitHelper _gitHelper;

        public TeamCity(ILog log, IGitHelper gitHelper)
        {
            _log = log;
            _gitHelper = gitHelper;
        }

        public bool IsRunningInBuildAgent()
        {
            var isRunningInBuildAgent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
            if (isRunningInBuildAgent)
            {
                _log.WriteLine("Executing inside a TeamCity build agent");
            }
            return isRunningInBuildAgent;
        }

        public bool IsBuildingAPullRequest(IRepository repository)
        {
            _gitHelper.NormalizeGitRepository(repository);
            var branchInfo = repository.Head.CanonicalName;
            var isBuildingAPullRequest = !string.IsNullOrEmpty(branchInfo) &&
                (branchInfo.ToLower().Contains("/pull/") || branchInfo.ToLower().Contains("/pull-requests/"));
            if (isBuildingAPullRequest)
            {
                _log.WriteLine("This is a pull request build for pull: " + CurrentPullRequestNo(repository.Head));
            }
            return isBuildingAPullRequest;
        }

        public int CurrentPullRequestNo(Branch branch)
        {
            return int.Parse(branch.CanonicalName.Split('/')[3]);
        }

        public void WriteBuildNumber(SemanticVersion nextBuildNumber)
        {
            TeamCityVersionWriter.WriteBuildNumber(nextBuildNumber);
        }

        public void WriteParameter(string variableName, string value)
        {
            TeamCityVersionWriter.WriteParameter(variableName.Replace('_', '.'), value);
        }
    }
}