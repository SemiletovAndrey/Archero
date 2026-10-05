using System.Collections.Generic;
using UnityEngine;

namespace Template.Features.Log
{
    public class LogService : ILogService
    {
        private List<ILogHandler> _loggers = new();
        
        public void Init()
        {
            Application.logMessageReceived += HandleLog;
        }

        public void AddLogHandler(ILogHandler logHandler)
        {
            _loggers.Add(logHandler);
        }

        public void RemoveLogHandler(ILogHandler logHandler)
        {
            _loggers.Remove(logHandler);
        }
        
        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            _loggers.ForEach(logger => logger.HandleLog(condition, stackTrace, type));
        }
    }
}