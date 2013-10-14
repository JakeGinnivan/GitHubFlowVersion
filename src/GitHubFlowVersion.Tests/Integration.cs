using System.Diagnostics;
using System.IO;
using System.Reflection;
using LibGit2Sharp;
using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class Integration
    {
        private readonly Repository _gitRepo;

        public Integration()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _gitRepo = new Repository(GitDirFinder.TreeWalkForGitDir(currentDirectory));
        }

        [Fact]
        public void GetNextVersionTests()
        {
            Assert.DoesNotThrow(()=>new NextVersionTxtFileFinder().GetNextVersion());
        }

        [Fact]
        public void LastTag()
        {
            var finder = new LastTaggedReleaseFinder(_gitRepo);
            Assert.DoesNotThrow(() => finder.GetVersion());
        }

        [Fact]
        public void NextVersion()
        {
            var calc = new NextSemverCalcualtor(new NextVersionTxtFileFinder(), new LastTaggedReleaseFinder(_gitRepo));

            var nextVersion = calc.NextVersion();

            Trace.WriteLine(nextVersion);
        }
    }
}