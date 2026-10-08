using Cysharp.Threading.Tasks;

namespace Template.Features.GameFlow.Scripts.Other.FeatureInitializer
{
    public interface IDeinitializableFeature
    {
        public UniTask DeinitFeature();
    }
}