using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Template.Features.ClassFactory
{
    public class ClassFactory : IClassFactory
    {
        private DiContainer _container;

        public ClassFactory(DiContainer container)
        {
            _container = container;
        }

        public void Init()
        {
        }

        public void SetContainer(DiContainer container)
        {
            _container = container;
        }

        public T Create<T>(IEnumerable<object> extraArgs = null)
        {
            extraArgs ??= Array.Empty<object>();

            return _container.Instantiate<T>(extraArgs);
        }

        public object Create(Type type, IEnumerable<object> extraArgs = null)
        {
            extraArgs ??= Array.Empty<object>();

            return _container.Instantiate(type, extraArgs);
        }

        public T Create<T>(Type type, IEnumerable<object> extraArgs = null) where T : class
        {
            extraArgs ??= Array.Empty<object>();

            return _container.Instantiate(type, extraArgs) as T;
        }

        public T CreateMonoBehaviour<T>(T prefab, Transform parent, bool dontDestroy = false) where T : Component
        {
            return CreateMonoBehaviour<T>(prefab.gameObject, parent, dontDestroy);
        }
        
        public T CreateMonoBehaviour<T>(T prefab, Vector3 position, Quaternion quaternion, Transform parent, bool dontDestroy = false) where T : Component
        {
            return CreateMonoBehaviour<T>(prefab.gameObject, position, quaternion, parent, dontDestroy);
        }

        public T CreateMonoBehaviour<T>(GameObject prefab, Transform parent, bool dontDestroy = false)
            where T : Component
        {
            T instance = _container
                .InstantiatePrefab(prefab, parent)
                .GetComponent<T>();

            if (dontDestroy)
            {
                Object.DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }

        public T CreateMonoBehaviour<T>(GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent,
            bool dontDestroy = false) where T : Component
        {
            T instance = _container
                .InstantiatePrefab(prefab, position, quaternion, parent)
                .GetComponent<T>();

            if (dontDestroy)
            {
                Object.DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }

        public T CreateMonoBehaviourAndBind<T>(T prefab, Transform parent, bool dontDestroy = false) where T : Component
        {
            T instance = CreateMonoBehaviour(prefab, parent, dontDestroy);

            _container.Bind<T>()
                .FromInstance(instance)
                .AsSingle();

            return instance;
        }

        public void BindFromGameObjectAsSingle<T>(GameObject prefab)
        {
            _container.Bind<T>().FromInstance(prefab.GetComponent<T>()).AsSingle();
        }

        public ConcreteIdArgConditionCopyNonLazyBinder BindSelfAsSingle<T>(T instance)
        {
            return _container.Bind<T>().FromInstance(instance).AsSingle();
        }

        public void Inject(GameObject gameObject)
        {
            _container.InjectGameObject(gameObject);
        }

        public void Inject<T>(T type)
        {
            _container.Inject(type);
        }

        public T CreateAndBind<T>()
        {
            _container.Bind<T>().AsSingle();

            return _container.Resolve<T>();
        }

        public object CreateAndBind(Type type)
        {
            _container.Bind(type).AsSingle();

            return _container.Resolve(type);
        }

        public T BindFromInstance<T>(T instance)
        {
            _container.Bind<T>().FromInstance(instance).AsSingle();

            return instance;
        }

        public List<T> CreateByBaseClass<T>(Predicate<T> extraPredicate = null) where T : class
        {
            Type parent = typeof(T);

            return parent.Assembly.GetTypes()
                .Where(type => !type.IsAbstract && parent.IsAssignableFrom(type))
                .Select(type => Create<T>(type))
                .Where(instance => extraPredicate is null || extraPredicate(instance))
                .ToList();
        }
    }
}