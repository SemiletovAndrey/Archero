using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore
{
    public readonly struct ProjectileArgs
    {
        public readonly Vector3 Direction;
        public readonly float Damage;

        public ProjectileArgs(Vector3 direction, float damage)
        {
            Direction = direction;
            Damage = damage;
        }
    }
}