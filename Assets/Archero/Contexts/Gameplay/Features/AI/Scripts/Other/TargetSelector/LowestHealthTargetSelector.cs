using System.Collections.Generic;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class LowestHealthTargetSelector : ITargetSelector
    {
        private readonly Entity _source;
        private readonly Transform _sourceTransform;

        public LowestHealthTargetSelector(Entity entity)
        {
            _source = entity;
            _sourceTransform = entity.Get<BodyTransform>().Transform;
        }

        public Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            Entity best = null;
            float bestHealth = float.MaxValue;
            float bestSqrDistance = float.MaxValue;

            foreach (Entity target in targets)
            {
                if (!IsValid(target))
                    continue;

                float health = target.Get<CurrentHealth>().Value;
                float sqrDistance = GetSqrDistanceTo(target);

                bool isBetter = health < bestHealth
                                || (Mathf.Approximately(health, bestHealth) && sqrDistance < bestSqrDistance);

                if (!isBetter)
                    continue;

                best = target;
                bestHealth = health;
                bestSqrDistance = sqrDistance;
            }

            return best;
        }

        private bool IsValid(Entity target)
        {
            if (target == _source)
                return false;

            if (!target.Has<TakeDamageRequest>() || !target.Has<CurrentHealth>() || !target.Has<BodyTransform>())
                return false;

            if (target.TryGet<CanApplyDamage>(out CanApplyDamage canApplyDamage) && !canApplyDamage.Evaluate())
                return false;

            return true;
        }

        private float GetSqrDistanceTo(Entity target)
        {
            return (_sourceTransform.position - target.Get<BodyTransform>().Transform.position).sqrMagnitude;
        }
    }
}