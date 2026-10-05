namespace Template.Features.ProjectInit
{
    public interface IFeatureFactory
    {
        public T CreateFeature<T>() where T : IBaseFeature;
    }
}