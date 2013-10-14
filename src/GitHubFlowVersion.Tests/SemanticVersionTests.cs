using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace GitHubFlowVersion.Tests
{
    public class SemanticVersionTests
    {
        [Theory]
        [ClassData(typeof(SemanticVersionCompareTests))]
        public void Compare(SemanticVersion first, SemanticVersion second, bool firstIsHigher)
        {
            Assert.True(firstIsHigher == first > second, string.Format("{0} > {1} != {2}", first, second, firstIsHigher));
        } 
    }

    public class SemanticVersionCompareTests : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { SemVer(0, 0, 1), new SemanticVersion(0, 0, 0), true };
            yield return new object[] { SemVer(0, 0, 1), new SemanticVersion(0, 0, 1), false };
            yield return new object[] { SemVer(0, 1, 0), new SemanticVersion(0, 1, 0), false };
            yield return new object[] { SemVer(0, 1, 0), new SemanticVersion(0, 2, 0), false };
            yield return new object[] { SemVer(0, 2, 0), new SemanticVersion(0, 1, 0), true };
            yield return new object[] { SemVer(1, 0, 0), new SemanticVersion(0, 0, 0), true };
            yield return new object[] { SemVer(0, 0, 0), new SemanticVersion(1, 0, 0), false };
            yield return new object[] { SemVer(1, 0, 0), new SemanticVersion(1, 0, 0), false };
        }

        private static SemanticVersion SemVer(int major, int minor, int patch)
        {
            return new SemanticVersion(major, minor, patch);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}