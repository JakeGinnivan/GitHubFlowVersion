using NSubstitute;
using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class NextSemverCalculatorTests
    {
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;
        private readonly INextVersionTxtFileFinder _txtFileVersion;
        private readonly NextSemverCalcualtor _sut;

        public NextSemverCalculatorTests()
        {
            _lastTaggedReleaseFinder = Substitute.For<ILastTaggedReleaseFinder>();
            _txtFileVersion = Substitute.For<INextVersionTxtFileFinder>();
            _sut = new NextSemverCalcualtor(_txtFileVersion, _lastTaggedReleaseFinder);
        }

        [Fact]
        public void SameVersionShouldBumpPatchVersion()
        {
            var currentVersion = new SemanticVersion(0, 1, 0);
            _lastTaggedReleaseFinder.GetVersion().Returns(currentVersion);
            _txtFileVersion.GetNextVersion().Returns(currentVersion);

            var nextVersion = _sut.NextVersion();

            Assert.Equal(new SemanticVersion(0, 1, 1), nextVersion);
        }

        [Fact]
        public void LesserVersionShouldBumpPatchVersionOfCurrentRelease()
        {
            var currentVersion = new SemanticVersion(0, 1, 0);
            var fileVersion = new SemanticVersion(0, 0, 1);
            _lastTaggedReleaseFinder.GetVersion().Returns(currentVersion);
            _txtFileVersion.GetNextVersion().Returns(fileVersion);

            var nextVersion = _sut.NextVersion();

            Assert.Equal(new SemanticVersion(0, 1, 1), nextVersion);
        }
    }
}