using System.Collections.Generic;
using System.Linq;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class NearestDamageableTargetSelector : ITargetSelector
    {
        private Entity _source;
        public Transform _sourceTransform;

        public NearestDamageableTargetSelector(Entity entity)
        {
            _source = entity;
            _sourceTransform = entity.Get<BodyTransform>().Transform;
        }
        
        public Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            IEnumerable<Entity> selectTargets = targets.Where(target =>
            {
                bool result = target.Has<TakeDamageRequest>();

                if (target.TryGet<CanApplyDamage>(out CanApplyDamage canApplyDamage))
                {
                    result = result && canApplyDamage.Evaluate();
                }

                result = result && (target != _source);
                
                return result;
            });
            
            if(!selectTargets.Any())
                return null;
            
            Entity closestTarget = selectTargets.First();

            float minDistance = GetDistanceTo(closestTarget);

            foreach (var target in selectTargets)
            {
                float distance = GetDistanceTo(target);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }
            
            return closestTarget;
        }

        private float GetDistanceTo(Entity entity)
        {
            return (_sourceTransform.position - entity.Get<BodyTransform>().Transform.position).magnitude;
        }
    }
}