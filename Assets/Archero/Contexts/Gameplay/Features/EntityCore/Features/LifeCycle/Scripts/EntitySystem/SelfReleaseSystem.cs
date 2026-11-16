using System;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.LifeCycle
{
    public class SelfReleaseSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        
        private MustSelfRelease _mustSelfRelease;

        private IDisposable _triggersDisposable;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _mustSelfRelease = entity.Get<MustSelfRelease>();

            _triggersDisposable = entity.Get<MustSelfReleaseTriggers>().Subscribe(Evaluate);
        }

        public void OnDispose()
        {
            _triggersDisposable?.Dispose();
        }

        private void Evaluate()
        {
            if (_mustSelfRelease.Evaluate())
            {
                _entity.Release();
            }
        }
    }
}