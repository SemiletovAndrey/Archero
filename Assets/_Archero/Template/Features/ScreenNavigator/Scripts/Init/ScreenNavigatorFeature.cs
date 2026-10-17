using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public class ScreenNavigatorFeature : BaseMonoBehaviourFeature
    {
        [SerializeField] private Transform _screenParent;
        
        protected override void InstallBindings()
        {
            BindAsSingle<IScreenNavigator, ScreenNavigator>();
            BindAsSingle<IScreenFactory, AddressablesScreenFactory>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<IScreenNavigator, ScreenNavigator>().Init();
            ResolveAs<IScreenFactory, AddressablesScreenFactory>().Init(_screenParent);
            return UniTask.CompletedTask;
        }
    }
}