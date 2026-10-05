using System;

namespace Template.Features.AppTime
{
    public interface IAppTimeService
    {
        public DateTime GetCurrentTime();
        
        public TimeSpan GetTimeDeltaTo(DateTime endTime);
        public TimeSpan GetTimeDeltaFrom(DateTime startTime);
        
        public float GetTimeDeltaInSecondsTo(DateTime endTime);
        public float GetTimeDeltaInSecondsFrom(DateTime startTime);
        
        public void AddProvider(ITimeProvider provider);
        public void RemoveProvider(ITimeProvider provider);
    }
}