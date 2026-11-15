using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Persistance.Features.EntityCore
{
    public sealed class EntityFactory : IEntityFactory
    {
        private readonly IEntityWorld _world;
        private readonly IMonoEntityFactory _views;
        private readonly DiContainer _container;
        private Dictionary<EntityType, IEntityRecipeBase> _recipes;

        public EntityFactory(IEntityWorld world, IMonoEntityFactory views, DiContainer container)
        {
            _world = world;
            _views = views;
            _container = container;
        }

        private Dictionary<EntityType, IEntityRecipeBase> Recipes => _recipes ??= BuildRecipeMap();

        private Dictionary<EntityType, IEntityRecipeBase> BuildRecipeMap()
        {
            var map = new Dictionary<EntityType, IEntityRecipeBase>();

            foreach (IEntityRecipeBase recipe in _container.ResolveAll<IEntityRecipeBase>())
            {
                if (!map.TryAdd(recipe.Type, recipe))
                    throw new InvalidOperationException($"Duplicate recipe for {recipe.Type}");
            }

            return map;
        }

        public Entity Create(EntityType type, Vector3 position)
        {
            IEntityRecipe recipe = GetRecipe<IEntityRecipe>(type);
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

        public Entity Create<TArgs>(EntityType type, Vector3 position, TArgs args)
        {
            IEntityRecipe<TArgs> recipe = GetRecipe<IEntityRecipe<TArgs>>(type);
            Entity entity = _world.Create(type);

            try
            {
                _views.Create(entity, position, type);
                recipe.Build(entity, args);
                _world.Add(entity);
            }
            catch
            {
                entity.Deactivate();
                throw;
            }

            return entity;
        }

        private T GetRecipe<T>(EntityType type) where T : class, IEntityRecipeBase
        {
            if (!Recipes.TryGetValue(type, out IEntityRecipeBase recipe))
                throw new InvalidOperationException($"No recipe for {type}");

            if (recipe is not T typed)
                throw new InvalidOperationException(
                    $"Recipe for {type} is {recipe.GetType().Name}, which does not implement {typeof(T).Name}");

            return typed;
        }
    }
}
