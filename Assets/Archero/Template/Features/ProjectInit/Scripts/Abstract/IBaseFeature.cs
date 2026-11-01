using System.Collections.Generic;

namespace Template.Features.ProjectInit
{
    public interface IBaseFeature
    {
        public string FeatureName { get; }

        public LinkedList<IInitializableFeature> GetInnerFeatures();
        public void Install();
    }
}