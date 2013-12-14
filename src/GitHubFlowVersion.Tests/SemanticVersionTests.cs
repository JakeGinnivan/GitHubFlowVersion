using GitVersion;
using Xunit;
using Xunit.Extensions;

namespace GitHubFlowVersion.Tests
{
    public class SemanticVersionTests
    {
        [Theory]
        [ClassData(typeof(SemanticVersionCompareTestsData))]
        public void Compare(SemanticVersion first, SemanticVersion second, bool firstIsHigher)
        {
            Assert.True(firstIsHigher == first > second, string.Format("{0} > {1} != {2}", first, second, firstIsHigher));
        }

        [Fact]
        public void BuildMetaDataIsIgnoredInEquality()
        {
            var first = new SemanticVersion(0, 1, 1, buildMetaData: 3);
            var second = new SemanticVersion(0, 1, 1, buildMetaData: 2);

            Assert.Equal(first, second);
            Assert.True(first == second);
        }

        [Fact]
        public void ToStringAppendsBuildMetaData()
        {
            var semver = new SemanticVersion(0, 1, 1, buildMetaData: 3);

            Assert.Equal("0.1.1+003", semver.ToString());
        }
    }
}