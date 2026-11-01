namespace Template.Features.Container
{
    public interface IContainerSystem
    {
        public TContainer GetContainer<TContainer>() where TContainer : IContainer;
        
        public void AddProvider(IContainerProvider provider);
        public void RemoveProvider(IContainerProvider provider);
    }
}