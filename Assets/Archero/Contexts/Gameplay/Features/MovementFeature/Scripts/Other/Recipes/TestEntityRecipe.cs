using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public sealed class TestEntityRecipe : IEntityRecipe
    {
        public EntityType Type => EntityType.Test;
        public void Build(Entity entity)
        {
            entity
                .Add(new MoveDirection())
                .Add(new MoveSpeed(10))
                .AddSystem(new RigidbodyMovementSystem());
        }
    }
}