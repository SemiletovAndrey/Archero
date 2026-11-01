using System;

namespace Template.Features.Config.Remote
{
    public class RemoteConfigNotFoundException : Exception
    {
        public RemoteConfigNotFoundException(Type configType) : base($"Remote config {configType.Name} was not found")
        {
            
        }
    }
}