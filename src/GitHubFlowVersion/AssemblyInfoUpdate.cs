using System;
using System.Collections.Generic;
using System.IO;

namespace GitHubFlowVersion
{
    public class AssemblyInfoUpdate : IDisposable
    {
        private readonly List<Action> _restoreBackupTasks = new List<Action>();
        private readonly List<Action> _cleanupBackupTasks = new List<Action>();

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
                _restoreBackupTasks.Add(() => fileSystem.Move(destFileName, sourceFileName, true));
                _cleanupBackupTasks.Add(() => fileSystem.DeleteFile(destFileName));

                var assemblyVersion = context.Variables[VariableProvider.AssemblyVersion];
                var assemblyInfoVersion = context.Variables[VariableProvider.AssemblyInformationalVersion];
                var assemblyFileVersion = context.Variables[VariableProvider.AssemblyFileVersion];
                var fileContents = fileSystem.ReadAllText(sourceFileName)
                    .RegexReplace(@"AssemblyVersion\(""\d+.\d+.\d+(.\d+|\*)?""\)", string.Format("AssemblyVersion(\"{0}\")", assemblyVersion))
                    .RegexReplace(@"AssemblyInformationalVersion\(""\d+.\d+.\d+(.\d+|\*)?""\)", string.Format("AssemblyInformationalVersion(\"{0}\")", assemblyInfoVersion))
                    .RegexReplace(@"AssemblyFileVersion\(""\d+.\d+.\d+(.\d+|\*)?""\)", string.Format("AssemblyFileVersion(\"{0}\")", assemblyFileVersion));

                fileSystem.WriteAllText(sourceFileName, fileContents);
            }
        }

        public void Dispose()
        {
            foreach (var restoreBackup in _restoreBackupTasks)
            {
                restoreBackup();
            }

            _cleanupBackupTasks.Clear();
            _restoreBackupTasks.Clear();
        }

        public void DoNotRestoreAssemblyInfo()
        {
            foreach (var cleanupBackupTask in _cleanupBackupTasks)
            {
                cleanupBackupTask();
            }
            _cleanupBackupTasks.Clear();
            _restoreBackupTasks.Clear();
        }
    }
}