using System;

namespace Template.Features.Log
{
    public class SimpleLogger : IDebugLogger
    {
        public void LogMessage(string message, DebugLogStyleData styleOverride = null)
        {
            DebugLogger.LogMessage(message);
        }

        public void LogWarning(string message, DebugLogStyleData styleOverride = null)
        {
            DebugLogger.LogWarning(message);
        }

        public void LogError(string message, DebugLogStyleData styleOverride = null)
        {
            DebugLogger.LogError(message);
        }

        public void LogException(Exception exception)
        {
            DebugLogger.LogException(exception);
        }
    }
}
