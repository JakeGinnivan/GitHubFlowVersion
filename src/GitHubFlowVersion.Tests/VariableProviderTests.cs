using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class VariableProviderTests
    {
        private readonly VariableProvider _sut;

        public VariableProviderTests()
        {
            _sut = new VariableProvider();
        }

        [Fact]
        public void VerifyReleaseVariables()
        {
            var version = new SemanticVersion(1, 2, 3, buildMetaData: 4);
            var variables = _sut.GetVariables(version);

            Assert.Equal("1.2.3.4", variables[VariableProvider.AssemblyFileVersion]);
            Assert.Equal("1.2.3+004", variables[VariableProvider.AssemblyInformationalVersion]);
            Assert.Equal("1.2.0.0", variables[VariableProvider.AssemblyVersion]);
            Assert.Equal("1.2.3+004", variables[VariableProvider.FullSemVer]);
            Assert.Equal("1.2.3", variables[VariableProvider.SemVer]);
            Assert.Equal("1.2.3.4", variables[VariableProvider.FourPartVersion]);
            Assert.Equal("1", variables[VariableProvider.Major]);
            Assert.Equal("2", variables[VariableProvider.Minor]);
            Assert.Equal("3", variables[VariableProvider.Patch]);
            Assert.Equal(null, variables[VariableProvider.Tag]);
            Assert.Equal("4", variables[VariableProvider.NumCommitsSinceRelease]);
        }

        [Fact]
        public void VerifypreReleaseVariables()
        {
            var version = new SemanticVersion(1, 2, 3, suffix: "beta", buildMetaData: 4);
            var variables = _sut.GetVariables(version);

            Assert.Equal("1.2.3.4", variables[VariableProvider.AssemblyFileVersion]);
            Assert.Equal("1.2.3-beta+004", variables[VariableProvider.AssemblyInformationalVersion]);
            Assert.Equal("1.2.0.0", variables[VariableProvider.AssemblyVersion]);
            Assert.Equal("1.2.3-beta+004", variables[VariableProvider.FullSemVer]);
            Assert.Equal("1.2.3-beta", variables[VariableProvider.SemVer]);
            Assert.Equal("1.2.3.4", variables[VariableProvider.FourPartVersion]);
            Assert.Equal("1", variables[VariableProvider.Major]);
            Assert.Equal("2", variables[VariableProvider.Minor]);
            Assert.Equal("3", variables[VariableProvider.Patch]);
            Assert.Equal("4", variables[VariableProvider.NumCommitsSinceRelease]);
            Assert.Equal("beta", variables[VariableProvider.Tag]);
        }
    }
}