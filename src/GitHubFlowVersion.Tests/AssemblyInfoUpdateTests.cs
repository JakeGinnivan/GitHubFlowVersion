using System.IO;
using NSubstitute;
using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class AssemblyInfoUpdateTests
    {
        private readonly IFileSystem _fileSystem;
        private readonly GitHubFlowVersionContext _context;

        public AssemblyInfoUpdateTests()
        {
            _context = new GitHubFlowVersionContext
            {
                Arguments = new GitHubFlowArguments(),
                Variables = new VariableProvider().GetVariables(new SemanticVersion(1, 2, 3, "beta", 4))
            };
            _fileSystem = Substitute.For<IFileSystem>();
        }

        [Fact]
        public void DoesNotUpdateWhenFlagNotPassed()
        {
            new GitHubFlowArguments().UpdateAssemblyInfo = false;

            using(new AssemblyInfoUpdate(_fileSystem, _context)){}

            _fileSystem.DidNotReceive().GetFiles(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<SearchOption>());
        }

        [Fact]
        public void CreatesBackupCopiesOfAssemblyInfoFiles()
        {
            _context.Arguments.UpdateAssemblyInfo = true;
            const string assemblyInfoFile = "C:\\Dir\\Properties\\AssemblyInfo.cs";
            _fileSystem
                .GetFiles(Arg.Any<string>(), Arg.Any<string>(), SearchOption.AllDirectories)
                .Returns(new[]{assemblyInfoFile});

            using (new AssemblyInfoUpdate( _fileSystem, _context)) { }

            _fileSystem.Received().Copy(assemblyInfoFile, assemblyInfoFile + ".bak", true);
        }

        [Fact]
        public void WhenDisposedBackupFilesOverwriteModified()
        {
            _context.Arguments.UpdateAssemblyInfo = true;
            const string assemblyInfoFile = "C:\\Dir\\Properties\\AssemblyInfo.cs";
            _fileSystem
                .GetFiles(Arg.Any<string>(), Arg.Any<string>(), SearchOption.AllDirectories)
                .Returns(new[] { assemblyInfoFile });

            using (new AssemblyInfoUpdate(_fileSystem, _context)) { }

            _fileSystem.Received().Move(assemblyInfoFile + ".bak", assemblyInfoFile, true);
        }



        [Fact]
        public void CallingDoNotRestoreBackupRemovesBackupFilesWithoutRestoring()
        {
            _context.Arguments.UpdateAssemblyInfo = true;
            const string assemblyInfoFile = "C:\\Dir\\Properties\\AssemblyInfo.cs";
            _fileSystem
                .GetFiles(Arg.Any<string>(), Arg.Any<string>(), SearchOption.AllDirectories)
                .Returns(new[] { assemblyInfoFile });

            using (var update = new AssemblyInfoUpdate(_fileSystem, _context))
            {
                update.DoNotRestoreAssemblyInfo();
            }

            _fileSystem.DidNotReceive().Move(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
            _fileSystem.Received().DeleteFile(assemblyInfoFile + ".bak");
        }

        [Fact]
        public void UpdatesVersionInAssemblyInfoFile()
        {
            _context.Arguments.UpdateAssemblyInfo = true;
            const string assemblyInfoFile = "C:\\Dir\\Properties\\AssemblyInfo.cs";
            _fileSystem
                .GetFiles(Arg.Any<string>(), Arg.Any<string>(), SearchOption.AllDirectories)
                .Returns(new[] { assemblyInfoFile });
            _fileSystem.ReadAllText(assemblyInfoFile).Returns(ExampleAssemblyInfoFile);

            using (new AssemblyInfoUpdate(_fileSystem, _context)) { }

            var expected = ExampleAssemblyInfoFile
                .Replace("AssemblyVersion(\"1.0.0.0\")", "AssemblyVersion(\"1.2.0.0\")")
                .Replace("AssemblyInformationalVersion(\"1.0.0.0\")", "AssemblyInformationalVersion(\"1.2.3-beta+004\")")
                .Replace("AssemblyFileVersion(\"1.0.0.0\")", "AssemblyFileVersion(\"1.2.3.4\")");
            _fileSystem.Received().WriteAllText(assemblyInfoFile, expected);
        }

        public const string ExampleAssemblyInfoFile =
            @"using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(""GitHubFlowVersion.Tests"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyProduct(""GitHubFlowVersion.Tests"")]
[assembly: AssemblyCopyright(""Copyright ©  2013"")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")]
[assembly: ComVisible(false)]

[assembly: Guid(""26faf5fa-7782-440f-a002-f0d4928bcf79"")]

[assembly: AssemblyVersion(""1.0.0.0"")]
[assembly: AssemblyInformationalVersion(""1.0.0.0"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]";
    }
}