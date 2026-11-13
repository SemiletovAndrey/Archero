using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public class EntityColliders : MonoEntityRegistrator
    {
        [SerializeField] private Collider[] _items;

        public IReadOnlyList<Collider> Items => _items;
        
        public override void Register(Entity entity) => entity.Add(this);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            Collider[] found = GetComponentsInChildren<Collider>(true);

            if (_items == null || !_items.SequenceEqual(found))
            {
                _items = found;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}