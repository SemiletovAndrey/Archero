using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.Config.Local;
using Template.Features.PrefabLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Persistance.Features.EntityCore
{
    public class MonoEntityFactory : IMonoEntityFactory, IDisposable
    {
        private readonly IClassFactory _classFactory;
        private readonly IPrefabLoader _prefabLoader;
        private readonly EntityConfig _entityConfig;
        private readonly IEntityLifeService _entityLifeService;

        private readonly Dictionary<EntityType, MonoEntity> _prefabs = new();
        private readonly List<AssetReferenceGameObject> _acquired = new();
        private readonly Dictionary<Entity, MonoEntity> _entityToMono = new();
        private bool _disposed;

        public MonoEntityFactory(IClassFactory classFactory, IPrefabLoader prefabLoader,
            ILocalConfigService configService, IEntityLifeService entityLifeService)
        {
            _classFactory = classFactory;
            _prefabLoader = prefabLoader;
            _entityLifeService = entityLifeService;

            _entityConfig = configService.GetConfig<EntityConfig>();
        }

        public async UniTask Init()
        {
            _entityLifeService.Released -= OnEntityReleased;
            _entityLifeService.Released += OnEntityReleased;

            await Preload();
        }

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

            if (_entityToMono.ContainsKey(entity))
                throw new InvalidOperationException("Entity already has a view.");

            MonoEntity view =
                _classFactory.CreateMonoBehaviour<MonoEntity>(prefab, position, Quaternion.identity, null);
            view.Setup(entity);

            _entityToMono.Add(entity, view);

            return view;
        }

        public bool TryGetView(Entity entity, out MonoEntity view)
        {
            return _entityToMono.TryGetValue(entity, out view);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _entityLifeService.Released -= OnEntityReleased;

            foreach (KeyValuePair<Entity, MonoEntity> pair in _entityToMono)
                DestroyView(pair.Key, pair.Value);

            _entityToMono.Clear();

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

        private void OnEntityReleased(Entity entity)
        {
            if (!_entityToMono.TryGetValue(entity, out MonoEntity view))
                return;

            _entityToMono.Remove(entity);
            DestroyView(entity, view);
        }

        private static void DestroyView(Entity entity, MonoEntity view)
        {
            if (view == null)
                return;

            try
            {
                view.Cleanup(entity);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            Object.Destroy(view.gameObject);
        }
    }
}