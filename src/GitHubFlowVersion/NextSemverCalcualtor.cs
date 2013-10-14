using System;

namespace GitHubFlowVersion
{
    public class NextSemverCalcualtor
    {
        private readonly INextVersionTxtFileFinder _nextVersionTxtFileFinder;
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;

        public NextSemverCalcualtor(
            INextVersionTxtFileFinder nextVersionTxtFileFinder,
            ILastTaggedReleaseFinder lastTaggedReleaseFinder)
        {
            _nextVersionTxtFileFinder = nextVersionTxtFileFinder;
            _lastTaggedReleaseFinder = lastTaggedReleaseFinder;
        }

        public SemanticVersion NextVersion()
        {
            SemanticVersion fileVersion = _nextVersionTxtFileFinder.GetNextVersion();
            SemanticVersion lastRelease = _lastTaggedReleaseFinder.GetVersion();
            if (fileVersion == lastRelease)
            {
                return new SemanticVersion(fileVersion.Major, fileVersion.Minor, fileVersion.Patch + 1);
            }

            throw new NotImplementedException();
        }
    }
}