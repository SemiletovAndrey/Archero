namespace Template.Features.Container
{
    public interface IContainerProvider
    {
        public bool TryGet<TContainer>(out TContainer container) where TContainer : IContainer;
    }
}