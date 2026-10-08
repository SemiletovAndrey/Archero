using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.GameFlow.Scripts.Other.FeatureInitializer
{
    public interface IFeatureInitializer
    {
        public UniTask InitAll(BaseFeatureSet featureSet, CancellationToken ct);
        public UniTask DeinitAll(BaseFeatureSet featureSet);
    }
}