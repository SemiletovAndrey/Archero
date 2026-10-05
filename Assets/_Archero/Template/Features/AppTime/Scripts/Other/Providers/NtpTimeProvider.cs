using System;
using Template.Features.Log;
using UnityEngine;

namespace Template.Features.AppTime
{
    public class NtpTimeProvider : ITimeProvider
    {
        private readonly IDebugLogger _logger;
        
        private DateTime _currentTime;
        private double _sinceStartTimeOnGettingServerTime;

        public NtpTimeProvider(IDebugLogger logger)
        {
            _logger = logger;
        }

        public void Init()
        {
            _currentTime = DateTime.UtcNow;

            using (NtpClient ntpClient = new NtpClient())
            {
                try
                {
                    _currentTime = ntpClient.GetNetworkTime();
                    _logger.LogMessage("Network Time: " + _currentTime.ToString("O"));
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError("Error retrieving network time: " + ex.Message);
                }
            }
            
            _sinceStartTimeOnGettingServerTime = Time.realtimeSinceStartupAsDouble;
        }

        public DateTime GetCurrentTime()
        {
            return _currentTime.AddSeconds(Time.realtimeSinceStartupAsDouble - _sinceStartTimeOnGettingServerTime);
        }
    }
}