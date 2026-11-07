using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.MovementFeature;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public class EntitiesFactory : IEntitiesFactory
    {
        private readonly IEntityLifeService _entityLifeService;
        private readonly IMonoEntityFactory _monoEntityFactory;

        public EntitiesFactory(IEntityLifeService entityLifeService, IMonoEntityFactory monoEntityFactory)
        {
            _entityLifeService = entityLifeService;
            _monoEntityFactory = monoEntityFactory;
        }

        public Entity CreateTestEntity(Vector3 position)
        {
            Entity entity = CreateEmpty();

            _monoEntityFactory.Create(entity, position, EntityType.None);
            
            entity.AddComponent(new MoveDirection() { Value = new ReactiveVariable<Vector3>(Vector3.forward) })
                .AddComponent(new MoveSpeed() { Value = new ReactiveVariable<float>(10) });

            entity.AddSystem(new MovementSystem());
            
            _entityLifeService.AddEntity(entity);

            return entity;
        }

        private Entity CreateEmpty() => new Entity();
    }
}