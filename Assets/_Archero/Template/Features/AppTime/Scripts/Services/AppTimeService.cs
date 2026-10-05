using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.AppTime
{
    public class AppTimeService : IAppTimeService
    {
        private static AppTimeService _instance;
        private List<ITimeProvider> _providers = new();

        public void Init()
        {
            _instance = this;
        }

        public DateTime GetCurrentTime()
        {
            return _providers.Last().GetCurrentTime();
        }

        public TimeSpan GetTimeDeltaTo(DateTime endTime)
        {
            return endTime - GetCurrentTime();
        }

        public TimeSpan GetTimeDeltaFrom(DateTime startTime)
        {
            return GetCurrentTime() - startTime;
        }

        public float GetTimeDeltaInSecondsTo(DateTime endTime)
        {
            return (float)GetTimeDeltaTo(endTime).TotalSeconds;
        }

        public float GetTimeDeltaInSecondsFrom(DateTime startTime)
        {
            return (float)GetTimeDeltaFrom(startTime).TotalSeconds;
        }

        public void AddProvider(ITimeProvider provider)
        {
            _providers.Add(provider);
        }

        public void RemoveProvider(ITimeProvider provider)
        {
            _providers.Remove(provider);
        }

        public static DateTime GetRealTimeNow()
        {
            if (_instance == null)
            {
                Debug.LogError("AppTimeService is not initialized");
                return DateTime.Now;
            }

            return _instance.GetCurrentTime();
        }
    }
}