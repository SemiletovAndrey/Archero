using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.LifeCycle
{
    public class SelfReleaseSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;

        private ReactiveVariable<bool> _isDead;
        private ReactiveVariable<bool> _inDeathProcess;
        private MustSelfRelease _mustSelfRelease;

        private IDisposable _subscription;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _isDead = entity.Get<IsDead>();
            _inDeathProcess = entity.Get<InDeathProcess>();
            _mustSelfRelease = entity.Get<MustSelfRelease>();

            _subscription = _inDeathProcess.Subscribe(OnDeadRelease);
        }

        public void OnDispose()
        {
            _subscription?.Dispose();
        }

        private void OnDeadRelease(bool oldValue, bool newValue)
        {
            if (_mustSelfRelease.Evaluate())
                _entity.Release();
        }
    }
}