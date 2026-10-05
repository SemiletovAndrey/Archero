using System;

namespace Template.Features.AppTime
{
    public interface ITimeProvider
    {
        public DateTime GetCurrentTime();
    }
}