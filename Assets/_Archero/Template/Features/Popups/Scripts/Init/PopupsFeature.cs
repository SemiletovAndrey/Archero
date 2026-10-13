using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;
using UnityEngine;

namespace Template.Features.Popups
{
    public class PopupsFeature : BaseMonoBehaviourFeature
    {
        [SerializeField] private Transform _uiContainer;
        protected override void InstallBindings()
        {
            BindAsSingle<IPopupSystem, PopupSystem>();
            BindAsSingle<IPopupFactory, AddressablesPopupFactory>();
        }

        protected override UniTask InitSystems()
        {
            ResolveAs<IPopupSystem, PopupSystem>().Init();
            ResolveAs<IPopupFactory, AddressablesPopupFactory>().Init(_uiContainer);
            return UniTask.CompletedTask;
        }
    }
}