using Contexts.Scripts.Utilities;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.EntityCore.Features.Energy;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Features.Config.Local;
using Template.Scripts.Utils;

namespace Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes
{
    public class TeleportatorEntityRecipe : IEntityRecipe
    {
        private readonly IColliderRegistryService _colliderRegistryService;
        private readonly ILocalConfigService _localConfigService;

        private TeleportatorEntityConfig _teleportatorEntityConfig;

        public TeleportatorEntityRecipe(IColliderRegistryService colliderRegistryService,
            ILocalConfigService localConfigService)
        {
            _colliderRegistryService = colliderRegistryService;
            _localConfigService = localConfigService;

            _teleportatorEntityConfig = _localConfigService.GetConfig<EntityDataConfig>()
                .GetConfig<TeleportatorEntityConfig>(Type);
        }

        public EntityType Type => EntityType.Teleportator;

        public void Build(Entity entity)
        {
            //Teleport
            entity
                .Add(new TeleportRadius(_teleportatorEntityConfig.TeleportRadius))
                .Add(new TeleportRequest())
                .Add(new TeleportEvent())
                .AddSystem(new RigidbodyTeleportSystem());

            //Energy
            entity
                .Add(new CurrentEnergy(_teleportatorEntityConfig.MaxEnergy))
                .Add(new MaxEnergy(_teleportatorEntityConfig.MaxEnergy))
                .Add(new EnergyRegenPercent(_teleportatorEntityConfig.EnergyRegenPercent))
                .Add(new EnergyRegenInterval(_teleportatorEntityConfig.EnergyRegenInterval))
                .Add(new EnergyRegenCurrentTime())
                .Add(new TeleportEnergyCost(_teleportatorEntityConfig.TeleportEnergyCost))
                .Add(new SpendEnergyRequest())
                .Add(new SpendEnergyEvent())
                .AddSystem(new SpendEnergySystem())
                .AddSystem(new TeleportEnergyCostSystem())
                .AddSystem(new EnergyRegenSystem())
                .AddSystem(new EnergyClampSystem());

            //hp
            entity
                .Add(new MaxHealth(_teleportatorEntityConfig.MaxHealth))
                .Add(new CurrentHealth(_teleportatorEntityConfig.MaxHealth))
                .Add(new IsDead(false))
                .Add(new InDeathProcess())
                .Add(new DeathProcessInitialTime(_teleportatorEntityConfig.DeathProcessInitialTime))
                .Add(new DeathProcessCurrentTime())
                .Add(new TakeDamageRequest())
                .Add(new TakeDamageEvent())
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem());

            entity
                .Add(new AttackProcessInitialTime(_teleportatorEntityConfig.AttackProcessInitialTime))
                .Add(new AttackProcessCurrentTime())
                .Add(new InAttackProcess())
                .Add(new AttackDelayTime(_teleportatorEntityConfig.AttackDelayTime))
                .Add(new StartAttackRequest())
                .Add(new StartAttackEvent())
                .Add(new EndAttackEvent())
                .Add(new AttackDelayEndEvent())
                .Add(new AttackCooldownInitialTime(_teleportatorEntityConfig.AttackCooldownInitialTime))
                .Add(new AttackCooldownCurrentTime())
                .Add(new InAttackCooldown())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new TeleportStartAttackSystem());

            //Splash
            entity
                .Add(new SplashAttackRadius(_teleportatorEntityConfig.SplashAttackRadius))
                .Add(new SplashAttackDamage(_teleportatorEntityConfig.SplashAttackDamage))
                .Add(new SplashTargetMask(Layers.CharactersMask))
                .Add(new SplashHitCollidersBuffer(64))
                .AddSystem(new SplashAttackSystem(_colliderRegistryService));

            CurrentEnergy currentEnergy = entity.Get<CurrentEnergy>();
            TeleportEnergyCost teleportCost = entity.Get<TeleportEnergyCost>();
            IsDead isDead = entity.Get<IsDead>();
            CurrentHealth currentHealth = entity.Get<CurrentHealth>();
            InDeathProcess inDeathProcess = entity.Get<InDeathProcess>();
            InAttackProcess inAttackProcess = entity.Get<InAttackProcess>();
            InAttackCooldown inAttackCooldown = entity.Get<InAttackCooldown>();

            CanSpendEnergy canSpendEnergy = new CanSpendEnergy();
            canSpendEnergy.Add(new FuncCondition(() => !isDead.Value));

            CanTeleport canTeleport = new CanTeleport();
            canTeleport.Add(new FuncCondition(() => currentEnergy.Value >= teleportCost.Value));

            MustDie mustDie = new MustDie();
            mustDie.Add(new FuncCondition(() => currentHealth.Value <= 0));

            MustSelfRelease mustSelfRelease = new MustSelfRelease();
            mustSelfRelease.Add(new FuncCondition(() => isDead.Value))
                .Add(new FuncCondition(() => inDeathProcess.Value == false));

            CanApplyDamage canApplyDamage = new CanApplyDamage();
            canApplyDamage.Add(new FuncCondition(() => !isDead.Value));

            MustDieTriggers mustDieTriggers = new MustDieTriggers();
            mustDieTriggers.On(currentHealth);

            MustSelfReleaseTriggers mustSelfReleaseTriggers = new MustSelfReleaseTriggers();
            mustSelfReleaseTriggers.On(isDead);
            mustSelfReleaseTriggers.On(inDeathProcess);

            CanStartAttack canStartAttack = new CanStartAttack();
            canStartAttack.Add(new FuncCondition(() => !isDead.Value));
            canStartAttack.Add(new FuncCondition(() => !inAttackProcess.Value));
            canStartAttack.Add(new FuncCondition(() => !inAttackCooldown.Value));

            entity
                .Add(canTeleport)
                .Add(canApplyDamage)
                .Add(mustDie)
                .Add(mustDieTriggers)
                .Add(mustSelfRelease)
                .Add(mustSelfReleaseTriggers)
                .Add(canStartAttack)
                .Add(canSpendEnergy);
        }
    }
}