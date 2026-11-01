using System;
using UnityEngine;

namespace Template.Features.AppTime
{
    public class NtpTimeProvider : ITimeProvider
    {
        private DateTime _currentTime;
        private double _sinceStartTimeOnGettingServerTime;
        
        public void Init()
        {
            _currentTime = DateTime.UtcNow;

            using (NtpClient ntpClient = new NtpClient())
            {
                try
                {
                    _currentTime = ntpClient.GetNetworkTime();
                }
                catch (InvalidOperationException ex)
                {
                    Debug.LogError(ex.Message);
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