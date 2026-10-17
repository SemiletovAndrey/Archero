using System.Collections.Generic;
using Template.Features.ProjectInit;

namespace Template.Features.GameFlow.Scripts
{
    public class GlobalFeatureSet : BaseFeatureSet
    {
        public GlobalFeatureSet(IEnumerable<IInitializableFeature> features) : base(features)
        {
        }
    }
}