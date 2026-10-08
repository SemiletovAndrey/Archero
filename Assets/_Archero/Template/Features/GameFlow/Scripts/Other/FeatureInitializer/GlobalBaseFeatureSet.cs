using System.Collections.Generic;
using Template.Features.ProjectInit;

namespace Template.Features.GameFlow.Scripts.Other.FeatureInitializer
{
    public class GlobalBaseFeatureSet : BaseFeatureSet
    {
        public GlobalBaseFeatureSet(IEnumerable<IInitializableFeature> features) : base(features)
        {
        }
    }
}