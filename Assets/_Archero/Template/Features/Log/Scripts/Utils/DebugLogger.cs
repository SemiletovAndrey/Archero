using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Template.Features.Log
{
    public class DebugLogger
    {
        [Conditional("ENABLE_LOGS")]
        public static void LogMessage(string message)
        {
            Debug.Log(message);
        }
        
        [Conditional("ENABLE_LOGS")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        [Conditional("ENABLE_LOGS")]
        public static void LogError(string message)
        {
            Debug.LogError(message);
        }
        
        [Conditional("ENABLE_LOGS")]
        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}