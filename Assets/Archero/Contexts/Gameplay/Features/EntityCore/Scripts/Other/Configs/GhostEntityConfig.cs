using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore
{
    [CreateAssetMenu(fileName = "Config_GhostEntity", menuName = "SO/Gameplay/EntityData/GhostEntity")]
    public class GhostEntityConfig : BaseEntityDataConfig
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 9;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 900;
        [field: SerializeField] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField] public float BodyContactDamage { get; private set; } = 50;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 2;
    }
}