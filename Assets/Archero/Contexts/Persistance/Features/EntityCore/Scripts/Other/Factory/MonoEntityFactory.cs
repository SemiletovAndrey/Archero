using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.Config.Local;
using Template.Features.PrefabLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Persistance.Features.EntityCore
{
    public class MonoEntityFactory : IMonoEntityFactory, IDisposable
    {
        private readonly IClassFactory _classFactory;
        private readonly IPrefabLoader _prefabLoader;
        private readonly EntityConfig _entityConfig;

        private readonly Dictionary<EntityType, MonoEntity> _prefabs = new();
        private readonly List<AssetReferenceGameObject> _acquired = new();
        private bool _disposed;

        public MonoEntityFactory(IClassFactory classFactory, IPrefabLoader prefabLoader,
            ILocalConfigService configService)
        {
            _classFactory = classFactory;
            _prefabLoader = prefabLoader;
            _entityConfig = configService.GetConfig<EntityConfig>();
        }

        public UniTask Init() => Preload();

        public UniTask Preload(CancellationToken ct = default)
        {
            var tasks = new List<UniTask>();
            foreach (EntityConfig.Entry entry in _entityConfig.Entries)
                tasks.Add(Load(entry, ct));

            return UniTask.WhenAll(tasks);
        }

        public MonoEntity Create(Entity entity, Vector3 position, EntityType type)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MonoEntityFactory));

            if (!_prefabs.TryGetValue(type, out MonoEntity prefab))
                throw new InvalidOperationException($"Prefab for '{type}' was not preloaded.");

            MonoEntity view =
                _classFactory.CreateMonoBehaviour<MonoEntity>(prefab, position, Quaternion.identity, null);

            entity.Add(new ViewLifetime(view.gameObject));
            view.Setup(entity);

            return view;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (AssetReferenceGameObject reference in _acquired)
                _prefabLoader.Release(reference);

            _acquired.Clear();
            _prefabs.Clear();
        }

        private async UniTask Load(EntityConfig.Entry entry, CancellationToken ct)
        {
            MonoEntity prefab = await _prefabLoader.Acquire<MonoEntity>(entry.Prefab, ct);

            if (_disposed)
            {
                _prefabLoader.Release(entry.Prefab);
                return;
            }

            _prefabs[entry.Type] = prefab;
            _acquired.Add(entry.Prefab);
        }
    }
}
