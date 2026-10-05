using System;

namespace Template.Features.Container
{
    public class ContainerNotFoundException : Exception
    {
        public ContainerNotFoundException(Type containerType) : base($"Container {containerType.Name} was not found")
        {
            
        }
    }
}