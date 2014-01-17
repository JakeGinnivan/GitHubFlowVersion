using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class SemanticVersionParserTests
    {
        [Fact]
        public void CanParseFourPartVersionNumber()
        {
            SemanticVersion semanticVersion;
            SemanticVersionParser.TryParse("1.2.3.4", out semanticVersion);

            Assert.Equal(1, semanticVersion.Major);
            Assert.Equal(2, semanticVersion.Minor);
            Assert.Equal(3, semanticVersion.Patch);
            Assert.Equal(4, semanticVersion.BuildMetaData);
        }
    }
}