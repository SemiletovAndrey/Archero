using Cysharp.Threading.Tasks;

namespace Template.Features.GameFlow.Scripts
{
    public interface IDeinitializableFeature
    {
        public UniTask DeinitFeature();
    }
}