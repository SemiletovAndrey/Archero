using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.LifeCycle
{
    public class DeathSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _isDead;
        
        private MustDie _mustDie;
        
        public void OnInit(Entity entity)
        {
            _isDead = entity.Get<IsDead>();
            _mustDie = entity.Get<MustDie>();
        }
        
        public void OnUpdate(float deltaTime)
        {
            if(_isDead.Value)
                return;
            
            if (_mustDie.Evaluate())
            {
                _isDead.Value = true;
            }
        }
    }
}