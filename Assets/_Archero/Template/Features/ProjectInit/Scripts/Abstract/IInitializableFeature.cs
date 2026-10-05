using Cysharp.Threading.Tasks;

namespace Template.Features.ProjectInit
{
    public interface IInitializableFeature : IBaseFeature
    {
        public UniTask InitFeature();
    }
}