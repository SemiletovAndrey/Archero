using System.Collections.Generic;
using Template.Features.ProjectInit;

namespace Template.Features.GameFlow.Scripts
{
    public abstract class BaseFeatureSet
    {
        private readonly List<IInitializableFeature> _features;
        private readonly List<IInitializableFeature> _started = new();
 
        protected BaseFeatureSet(IEnumerable<IInitializableFeature> features)
        {
            _features = new List<IInitializableFeature>(features);
        }
 
        public IReadOnlyList<IInitializableFeature> Features => _features;
 
        internal IReadOnlyList<IInitializableFeature> Started => _started;
 
        internal bool TryMarkStarted(IInitializableFeature feature)
        {
            if (_started.Contains(feature))
            {
                return false;
            }
 
            _started.Add(feature);
            return true;
        }
 
        internal void ClearStarted()
        {
            _started.Clear();
        }

    }
}