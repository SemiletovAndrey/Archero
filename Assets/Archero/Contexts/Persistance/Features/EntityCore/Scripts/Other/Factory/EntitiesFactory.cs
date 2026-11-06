using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.MovementFeature;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public class EntitiesFactory : IEntitiesFactory
    {
        private IEntityLifeService _entityLifeService;

        public EntitiesFactory(IEntityLifeService entityLifeService)
        {
            _entityLifeService = entityLifeService;
        }

        public Entity CreateTestEntity()
        {
            Entity entity = CreateEmpty();

            entity.AddComponent(new MoveDirection() { Value = new ReactiveVariable<Vector3>(Vector3.forward) })
                .AddComponent(new MoveSpeed() { Value = new ReactiveVariable<float>(10) });

            entity.AddSystem(new MovementSystem());
            
            _entityLifeService.AddEntity(entity);

            return entity;
        }

        private Entity CreateEmpty() => new Entity();
    }
}