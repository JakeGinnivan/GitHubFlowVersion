using System;
using System.Collections.Generic;
using System.IO;

namespace GitHubFlowVersion
{
    public class AssemblyInfoUpdate : IDisposable
    {
        private readonly List<Action> _cleanupTasks = new List<Action>();

        public AssemblyInfoUpdate(IFileSystem fileSystem, GitHubFlowVersionContext context)
        {
            if (!context.Arguments.UpdateAssemblyInfo) return;

            var assemblyInfoFiles = fileSystem.GetFiles(context.RepositoryRoot, "AssemblyInfo.cs",
                SearchOption.AllDirectories);

            foreach (var assemblyInfoFile in assemblyInfoFiles)
            {
                var destFileName = assemblyInfoFile + ".bak";
                var sourceFileName = assemblyInfoFile;
                fileSystem.Copy(assemblyInfoFile, destFileName, true);
                _cleanupTasks.Add(() => fileSystem.Move(destFileName, sourceFileName, true));

                var assemblyVersion = context.Variables[VariableProvider.AssemblyVersion];
                var assemblyInfoVersion = context.Variables[VariableProvider.AssemblyInformationalVersion];
                var assemblyFileVersion = context.Variables[VariableProvider.AssemblyFileVersion];
                var fileContents = fileSystem.ReadAllText(sourceFileName)
                .Replace("AssemblyVersion(\"1.0.0.0\")", string.Format("AssemblyVersion(\"{0}\")", assemblyVersion))
                .Replace("AssemblyInformationalVersion(\"1.0.0.0\")", string.Format("AssemblyInformationalVersion(\"{0}\")", assemblyInfoVersion))
                .Replace("AssemblyFileVersion(\"1.0.0.0\")", string.Format("AssemblyFileVersion(\"{0}\")", assemblyFileVersion));

                fileSystem.WriteAllText(sourceFileName, fileContents);
            }
        }

        public void Dispose()
        {
            foreach (var cleanupTask in _cleanupTasks)
            {
                cleanupTask();
            }
        }
    }
}