using System;

namespace Template.Features.AppTime
{
    public class LocalTimeProvider : ITimeProvider
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }
    }
}