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

        protected override UniTask<GameObject> AcquirePrefab(PopupEntryBase entry)
        {
            if (entry is not PopupEntry direct)
                throw new System.NotSupportedException(
                    $"{nameof(InstantiatePopupFactory)} supports only {nameof(PopupEntry)}, got {entry.GetType().Name}.");

            return UniTask.FromResult(direct.Prefab);
        }

        protected override void ReleasePrefab(PopupEntryBase entry)
        {
            
        }

    }
}