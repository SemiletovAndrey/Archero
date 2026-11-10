using System;
using System.Collections.Generic;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public sealed class EntityFactory : IEntityFactory
    {
        private readonly IEntityWorld _world;
        private readonly IMonoEntityFactory _views;
        private readonly Dictionary<EntityType, IEntityRecipe> _recipes = new();

        public EntityFactory(IEntityWorld world, IMonoEntityFactory views, List<IEntityRecipe> recipes)
        {
            _world = world;
            _views = views;

            foreach (IEntityRecipe recipe in recipes)
            {
                if (!_recipes.TryAdd(recipe.Type, recipe))
                    throw new InvalidOperationException($"Duplicate recipe for {recipe.Type}");
            }
        }

        public Entity Create(EntityType type, Vector3 position)
        {
            if (!_recipes.TryGetValue(type, out IEntityRecipe recipe))
                throw new InvalidOperationException($"No recipe for {type}");

            Entity entity = _world.Create(type);

            try
            {
                _views.Create(entity, position, type);
                recipe.Build(entity);
                _world.Add(entity);
            }
            catch
            {
                entity.Deactivate();
                throw;
            }

            return entity;
        }
    }
}
