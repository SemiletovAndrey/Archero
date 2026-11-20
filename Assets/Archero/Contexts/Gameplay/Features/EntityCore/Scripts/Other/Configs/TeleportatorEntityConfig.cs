using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore
{
    [CreateAssetMenu(fileName = "Config_TeleportatorEntity", menuName = "SO/Gameplay/EntityData/TeleportatorEntity")]
    public class TeleportatorEntityConfig : BaseEntityDataConfig
    {
        [field: SerializeField] public float TeleportRadius { get; private set; } = 5;
        [field: SerializeField] public float MaxEnergy { get; private set; } = 100;
        [field: SerializeField] public float EnergyRegenPercent { get; private set; } = 0.1f;
        [field: SerializeField] public float EnergyRegenInterval { get; private set; } = 1f;
        [field: SerializeField] public float TeleportEnergyCost { get; private set; } = 30f;
        [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
        [field: SerializeField] public float DeathProcessInitialTime { get; private set; } = 2f;
        [field: SerializeField] public float AttackProcessInitialTime { get; private set; } = 1f;
        [field: SerializeField] public float AttackDelayTime { get; private set; } = 0.5f;
        [field: SerializeField] public float AttackCooldownInitialTime { get; private set; } = 1f;
        [field: SerializeField] public float SplashAttackRadius { get; private set; } = 4f;
        [field: SerializeField] public float SplashAttackDamage { get; private set; } = 30f;
    }
}