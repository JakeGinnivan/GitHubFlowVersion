namespace GitVersion
{
    // todo: fix spelling mistake
    public class NextSemverCalculator : INextSemverCalculator
    {
        private readonly INextVersionTxtFileFinder _nextVersionTxtFileFinder;
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;

        public NextSemverCalculator(
            INextVersionTxtFileFinder nextVersionTxtFileFinder,
            ILastTaggedReleaseFinder lastTaggedReleaseFinder)
        {
            _nextVersionTxtFileFinder = nextVersionTxtFileFinder;
            _lastTaggedReleaseFinder = lastTaggedReleaseFinder;
        }

        public SemanticVersion NextVersion()
        {
            SemanticVersion lastRelease = _lastTaggedReleaseFinder.GetVersion().SemVer;
            SemanticVersion fileVersion = _nextVersionTxtFileFinder.GetNextVersion(lastRelease);
            if (fileVersion <= lastRelease)
            {
                return new SemanticVersion(lastRelease.Major, lastRelease.Minor, lastRelease.Patch + 1);
            }

            return fileVersion;
        }
    }
}