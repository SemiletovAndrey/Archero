using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public class InstantiateScreenFactory : ScreenFactoryBase
    {
        public InstantiateScreenFactory(IClassFactory classFactory, IUILayersService layersService)
            : base(classFactory, layersService)
        {
        }

        protected override UniTask<GameObject> AcquirePrefab(ScreenEntryBase entry)
        {
            if (entry is not ScreenEntry direct)
                throw new System.NotSupportedException(
                    $"{nameof(InstantiateScreenFactory)} supports only {nameof(ScreenEntry)}, got {entry.GetType().Name}.");

            return UniTask.FromResult(direct.Prefab);
        }

        protected override void ReleasePrefab(ScreenEntryBase entry)
        {
            
        }
    }
}