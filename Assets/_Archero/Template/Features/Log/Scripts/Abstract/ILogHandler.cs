using UnityEngine;

namespace Template.Features.Log
{
    public interface ILogHandler
    {
        public void HandleLog(string condition, string stackTrace, LogType type);
    }
}