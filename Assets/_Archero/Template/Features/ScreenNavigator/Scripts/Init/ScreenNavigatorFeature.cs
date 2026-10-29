using Cysharp.Threading.Tasks;
using Template.Features.Presenters;
using Template.Features.ProjectInit;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class ScreenNavigatorFeature<TRoot> : BaseMonoBehaviourFeature where TRoot : NavigationRoot
    {
        [SerializeField] private Transform _screenParent;
        
        protected override void InstallBindings()
        {
            BindAsSingle<IMainNavigationItem, TRoot>();

            BindAsSingle<IScreenNavigator, ScreenNavigator>();
            BindAsSingle<IScreenFactory, AddressablesScreenFactory>();
            BindAsSingle<IPresenterFactory, PresenterFactory>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<IScreenNavigator, ScreenNavigator>().Init();
            ResolveAs<IScreenFactory, AddressablesScreenFactory>().Init(_screenParent);
            return UniTask.CompletedTask;
        }
    }
}