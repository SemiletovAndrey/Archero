using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore
{
    [CreateAssetMenu(fileName = "Config_HeroEntity", menuName = "SO/Gameplay/EntityData/HeroEntity")]
    public class HeroEntityConfig : BaseEntityDataConfig
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 9;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 900;
        [field: SerializeField] public float AttackProcessTime { get; private set; } = 1.5f;
        [field: SerializeField] public float AttackDelayTime { get; private set; } = 0.75f;
        [field: SerializeField] public float AttackCooldown { get; private set; } = 1f;
        [field: SerializeField] public float InstantAttackDamage { get; private set; } = 50f;
        [field: SerializeField] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField] public float DeathProcessInitialTime { get; private set; } = 2;
    }
}