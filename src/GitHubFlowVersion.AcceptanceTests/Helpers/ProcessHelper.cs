using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests.Helpers
{
    public static class ProcessHelper
    {
        private static volatile object _lockObject = new object();

        // http://social.msdn.microsoft.com/Forums/en/netfxbcl/thread/f6069441-4ab1-4299-ad6a-b8bb9ed36be3
        public static Process Start(ProcessStartInfo startInfo)
        {
            Process process;

            lock (_lockObject)
            {
                using (new ChangeErrorMode(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox))
                {
                    process = Process.Start(startInfo);
                    process.PriorityClass = ProcessPriorityClass.Idle;
                }
            }

            return process;
        }

        public static void ExitedWithoutError(this Process process)
        {
            if (process.ExitCode != 0)
            {
                Trace.WriteLine(process.StandardOutput.ReadToEnd());
                Trace.WriteLine(process.StandardError.ReadToEnd());
                Assert.Equal(0, process.ExitCode);
            }
        }

        [Flags]
        public enum ErrorModes
        {
            Default = 0x0,
            FailCriticalErrors = 0x1,
            NoGpFaultErrorBox = 0x2,
            NoAlignmentFaultExcept = 0x4,
            NoOpenFileErrorBox = 0x8000
        }

        public struct ChangeErrorMode : IDisposable
        {
            private readonly int _oldMode;

            public ChangeErrorMode(ErrorModes mode)
            {
                _oldMode = SetErrorMode((int)mode);
            }

            void IDisposable.Dispose() { SetErrorMode(_oldMode); }

            [DllImport("kernel32.dll")]
            private static extern int SetErrorMode(int newMode);
        }
    }
}
