using System.Collections.Generic;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public class MonoEntity : MonoBehaviour
    {
        [Tooltip("Unity-компоненты, которые становятся компонентами сущности под своим конкретным типом")]
        [SerializeField] private Component[] _exposed;

        [Tooltip("Заполняется автоматически в редакторе")]
        [SerializeField] private MonoEntityRegistrator[] _registrators;

        private Entity _linkedEntity;
        
        public Entity LinkedEntity => _linkedEntity;
        
        public void Setup(Entity entity)
        {
            _linkedEntity = entity;
            
            entity.Add(this);
            entity.Add(transform);

            if (_exposed != null)
            {
                foreach (Component component in _exposed)
                {
                    if (component == null || component == this || component is Transform)
                        continue;

                    entity.Add(component.GetType(), component);
                }
            }

            if (_registrators != null)
            {
                foreach (MonoEntityRegistrator registrator in _registrators)
                    registrator.Register(entity);
            }
        }

        public void Cleanup()
        {
            _linkedEntity = null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            MonoEntityRegistrator[] found = GetComponentsInChildren<MonoEntityRegistrator>(true);

            if (!SameContent(_registrators, found))
            {
                _registrators = found;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        [ContextMenu("Expose Rigidbody / Colliders / Animator")]
        private void ExposeCommon()
        {
            var list = new List<Component>();
            list.AddRange(GetComponentsInChildren<Rigidbody>(true));
            list.AddRange(GetComponentsInChildren<Collider>(true));
            list.AddRange(GetComponentsInChildren<Animator>(true));

            UnityEditor.Undo.RecordObject(this, "Expose components");
            _exposed = list.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private static bool SameContent(MonoEntityRegistrator[] a, MonoEntityRegistrator[] b)
        {
            if (a == null || a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
#endif
    }
}
