using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.LifeCycle
{
    public class SelfReleaseSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;
        
        private MustSelfRelease _mustSelfRelease;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _mustSelfRelease = entity.Get<MustSelfRelease>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_mustSelfRelease.Evaluate())
            {
                _entity.Release();
            }
        }
    }
}