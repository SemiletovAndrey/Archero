using System;
using Template.Features.ClassFactory;
using Template.Features.Config.Local;
using Template.Features.PrefabLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Persistance.Features.EntityCore
{
    public class MonoEntityFactory
    {
        private readonly IClassFactory _classFactory;
        private readonly IPrefabLoader _prefabLoader;

        private EntityConfig _entityConfig;
        
        

        public MonoEntityFactory(IClassFactory classFactory, IPrefabLoader prefabLoader, ILocalConfigService configService)
        {
            _classFactory = classFactory;
            _prefabLoader = prefabLoader;
            
            _entityConfig = configService.GetConfig<EntityConfig>();
        }

        public MonoEntity Create(Entity entity, Vector3 position, EntityType type)
        {
            AssetReferenceGameObject reference = _entityConfig.Get(type);
            
            if (!_prefabLoader.TryGet(reference, out MonoEntity prefab))
                throw new InvalidOperationException("Prefab was not preloaded.");

            MonoEntity view = _classFactory.CreateMonoBehaviour<MonoEntity>(prefab, position, Quaternion.identity, null);
            view.Setup(entity);
            return view;
        }
    }
}