using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class NextVersionTxtFinderTests
    {
        [Fact]
        public void GetNextVersionTests()
        {
            Assert.DoesNotThrow(()=>new NextVersionTxtFileFinder().GetNextVersion());
        } 
    }
}