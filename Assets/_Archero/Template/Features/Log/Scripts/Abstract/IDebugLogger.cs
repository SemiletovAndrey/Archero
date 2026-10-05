using System;

namespace Template.Features.Log
{
    public interface IDebugLogger
    {
        public void LogMessage(string message, DebugLogStyleData styleOverride = null);
        public void LogWarning(string message, DebugLogStyleData styleOverride = null);
        public void LogError(string message, DebugLogStyleData styleOverride = null);
        public void LogException(Exception exception);
    }
}
