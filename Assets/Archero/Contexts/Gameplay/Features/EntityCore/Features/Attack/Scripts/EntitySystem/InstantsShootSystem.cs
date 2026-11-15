using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class InstantsShootSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _attackDelayEndEvent;
        private Transform _shootPoint;
        private ReactiveVariable<float> _instantAttackDamage;

        private IDisposable _attackEndDisposable;
        
        private readonly IEntityFactory _entityFactory;

        public InstantsShootSystem(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.Get<AttackDelayEndEvent>();
            _shootPoint = entity.Get<ShootPoint>().Value;
            _instantAttackDamage = entity.Get<InstantAttackDamage>();

            _attackEndDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
        }
        
        public void OnDispose()
        {
            _attackEndDisposable?.Dispose();
        }

        private void OnAttackDelayEnd()
        {
            ProjectileArgs projectileArgs = new ProjectileArgs(_shootPoint.forward, _instantAttackDamage.Value);
            _entityFactory.Create(EntityType.Projectile, _shootPoint.position, projectileArgs);
        }
    }
}