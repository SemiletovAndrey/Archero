using System;

namespace Template.Features.Config.Local
{
    public class LocalConfigNotFoundException: Exception
    {
        public LocalConfigNotFoundException(Type configType) : base($"Scriptable config {configType.Name} was not found")
        {
            
        }
    }
}