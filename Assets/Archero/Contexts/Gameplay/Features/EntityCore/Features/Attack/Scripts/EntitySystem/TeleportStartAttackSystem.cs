using System;
using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class TeleportStartAttackSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _startAttackRequest;
        private IDisposable _teleportDisposable;

        public void OnInit(Entity entity)
        {
            _startAttackRequest = entity.Get<StartAttackRequest>();
            
            _teleportDisposable = entity.Get<TeleportEvent>().Subscribe(OnTeleported);
        }

        public void OnDispose()
        {
            _teleportDisposable?.Dispose();
        }

        private void OnTeleported()
        {
            _startAttackRequest.Invoke();
        }
    }
}