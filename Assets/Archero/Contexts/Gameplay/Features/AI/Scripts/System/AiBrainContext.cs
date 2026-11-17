using System;
using System.Collections.Generic;
using Persistance.Features.EntityCore;
using Persistance.Features.GameLoopFeature;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class AiBrainContext : IDisposable, IAiBrainContext, IGameUpdatable
    {
        private readonly List<EntityToBrain> _entityToBrains = new List<EntityToBrain>();
        
        public void SetFor(Entity entity, IBrain brain)
        {
            foreach (var entityToBrain in _entityToBrains)
            {
                if (entityToBrain.Entity == entity)
                {
                    entityToBrain.Brain.Disable();
                    entityToBrain.Brain.Dispose();
                    entityToBrain.Brain = brain;
                    entityToBrain.Brain.Enable();
                    return;
                }
            }

            _entityToBrains.Add(new EntityToBrain(entity, brain));
            brain.Enable();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _entityToBrains.Count; i++)
            {
                if (_entityToBrains[i].Entity.IsAlive == false)
                {
                    int lasIndex = _entityToBrains.Count - 1;
                    _entityToBrains[i].Brain.Disable();
                    _entityToBrains[i] = _entityToBrains[lasIndex];
                    _entityToBrains.RemoveAt(lasIndex);
                    i--;
                    continue;
                }

                _entityToBrains[i].Brain.Update(deltaTime);
            }
        }
        
        public void Dispose()
        {
            foreach (var entityToBrain in _entityToBrains)
            {
                entityToBrain.Brain.Dispose();
            }
            _entityToBrains.Clear();
        }

        private class EntityToBrain
        {
            public Entity Entity;
            public IBrain Brain;

            public EntityToBrain(Entity entity, IBrain brain)
            {
                Entity = entity;
                Brain = brain;
            }
        }
    }
}