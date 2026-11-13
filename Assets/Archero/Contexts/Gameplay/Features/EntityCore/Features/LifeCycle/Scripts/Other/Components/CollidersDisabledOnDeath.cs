using System.Collections.Generic;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.LifeCycle
{
    public sealed class CollidersDisabledOnDeath : MonoEntityRegistrator
    {
        [SerializeField] private Collider[] _items;

        public IReadOnlyList<Collider> Items => _items;

        public override void Register(Entity entity) => entity.Add(this);
    }
}