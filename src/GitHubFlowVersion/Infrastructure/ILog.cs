using System;

namespace GitVersion.Infrastructure
{
    public interface ILog
    {
        void WriteLine(string value);
        void WriteLine(string format, params object[] args);
        void WriteLine();
        void WriteErrorLine(Exception exception);
        void WriteErrorLine(string error);
    }
}