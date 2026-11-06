using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Template.Features.ClassFactory
{
    public interface IClassFactory
    {
        public void Init();

        public void SetContainer(DiContainer container);

        public T Create<T>(IEnumerable<object> extraArgs = null);
        public object Create(Type type, IEnumerable<object> extraArgs = null);
        public T Create<T>(Type type, IEnumerable<object> extraArgs = null) where T : class;

        public T CreateMonoBehaviour<T>(T prefab, Transform parent, bool dontDestroy = false) where T : Component;
        public T CreateMonoBehaviour<T>(GameObject prefab, Transform parent, bool dontDestroy = false) where T : Component;
        public T CreateMonoBehaviour<T>(GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent,
            bool dontDestroy = false) where T : Component;
        public T CreateMonoBehaviour<T>(T prefab, Vector3 position, Quaternion quaternion, Transform parent,
            bool dontDestroy = false) where T : Component;
        public T CreateMonoBehaviourAndBind<T>(T prefab, Transform parent, bool dontDestroy = false) where T : Component;
        
        public T CreateAndBind<T>();
        public object CreateAndBind(Type type);
        public T BindFromInstance<T>(T instance);

        public void BindFromGameObjectAsSingle<T>(GameObject prefab);
        public ConcreteIdArgConditionCopyNonLazyBinder BindSelfAsSingle<T>(T instance);
        
        public void Inject(GameObject gameObject);
        public void Inject<T>(T type);
        
        public List<T> CreateByBaseClass<T>(Predicate<T> extraPredicate = null) where T : class;
    }
}
