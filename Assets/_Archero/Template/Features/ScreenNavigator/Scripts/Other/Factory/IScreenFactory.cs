using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public interface IScreenFactory
    {
        public void Init(Transform parent);
        
        public UniTask<T> Create<T>(ScreenType screenType, GameObject prefab, Transform parent = null) where T : Component;
        public void Destroy<T>(T instance, ScreenType screenType) where T : Component;
    }
}