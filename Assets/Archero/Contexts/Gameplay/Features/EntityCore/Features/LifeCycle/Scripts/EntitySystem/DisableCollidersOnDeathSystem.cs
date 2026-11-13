using System;
using System.Collections.Generic;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.LifeCycle
{
    public class DisableCollidersOnDeathSystem : IInitializableSystem, IDisposableSystem
    {
        private IReadOnlyList<Collider> _colliderOnDeath = new List<Collider>();
        private ReactiveVariable<bool> _isDead;

        private IDisposable _colliderOnDeathDisposable;

        public void OnInit(Entity entity)
        {
            _colliderOnDeath = entity.Get<CollidersDisabledOnDeath>().Items;
            _isDead = entity.Get<IsDead>();

            _colliderOnDeathDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }

        public void OnDispose()
        {
            _colliderOnDeathDisposable.Dispose();
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            if (isDead)
            {
                foreach (var collider in _colliderOnDeath)
                {
                    collider.enabled = false;
                }
            }
        }
    }
}