using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.Popups
{
    public class InstantiatePopupFactory : PopupFactoryBase
    {
        public InstantiatePopupFactory(IClassFactory classFactory, IUILayersService layersService)
            : base(classFactory, layersService)
        {
        }

        public override bool Supports(PopupEntryBase entry)
        {
            return entry is PopupEntry;
        }

        public override UniTask<T> Create<T>(PopupEntryBase entry)
        {
            EnsureReady(entry);

            if (entry is not PopupEntry direct)
                throw new System.NotSupportedException(
                    $"{nameof(InstantiatePopupFactory)} supports only {nameof(PopupEntry)}, got {entry.GetType().Name}.");

            return UniTask.FromResult(InstantiatePopup<T>(direct.Prefab, entry));
        }

        public override void Destroy(BasePopup popup)
        {
            if (popup != null)
                Object.Destroy(popup.gameObject);
        }

    }
}