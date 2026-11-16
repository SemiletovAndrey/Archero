using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.LifeCycle
{
    public class DeathSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<bool> _isDead;
        
        private MustDie _mustDie;

        private IDisposable _triggersDisposable;
        
        public void OnInit(Entity entity)
        {
            _isDead = entity.Get<IsDead>();
            _mustDie = entity.Get<MustDie>();

            _triggersDisposable = entity.Get<MustDieTriggers>().Subscribe(Evaluate);
        }

        public void OnDispose()
        {
            _triggersDisposable?.Dispose();
        }
        
        private void Evaluate()
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