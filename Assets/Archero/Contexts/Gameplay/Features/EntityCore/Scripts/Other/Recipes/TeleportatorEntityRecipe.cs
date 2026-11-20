using Gameplay.Features.ApplyDamage;
using Gameplay.Features.ContactTakeDamage;
using Gameplay.Features.EntityCore.Features.Energy;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes
{
    public class TeleportatorEntityRecipe : IEntityRecipe
    {
        public EntityType Type => EntityType.Teleportator;

        private IColliderRegistryService _colliderRegistryService;

        public TeleportatorEntityRecipe(IColliderRegistryService colliderRegistryService)
        {
            _colliderRegistryService = colliderRegistryService;
        }

        public void Build(Entity entity)
        {
            //Teleport
            entity
                .Add(new TeleportRadius(5))
                .Add(new TeleportRequest())
                .Add(new TeleportEvent())
                .AddSystem(new RigidbodyTeleportSystem())
                .AddSystem(new BodyContactsDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_colliderRegistryService));
            
            //Energy
            entity
                .Add(new CurrentEnergy(100))
                .Add(new MaxEnergy(100))
                .Add(new EnergyRegenPercent(0.1f))
                .Add(new EnergyRegenInterval(1f))
                .Add(new EnergyRegenCurrentTime())
                .Add(new TeleportEnergyCost(30))
                .Add(new SpendEnergyRequest())
                .Add(new SpendEnergyEvent())
                .AddSystem(new SpendEnergySystem())
                .AddSystem(new TeleportEnergyCostSystem())
                .AddSystem(new EnergyRegenSystem())
                .AddSystem(new EnergyClampSystem());
            
            //hp
            entity
                .Add(new MaxHealth(100))
                .Add(new CurrentHealth(100))
                .Add(new IsDead(false))
                .Add(new InDeathProcess())
                .Add(new DeathProcessInitialTime(2))
                .Add(new DeathProcessCurrentTime())
                .Add(new TakeDamageRequest())
                .Add(new TakeDamageEvent())
                .Add(new ContactDetectingMask(1 << LayerMask.NameToLayer("Characters")))
                .Add(new ContactCollidersBuffer(64))
                .Add(new ContactEntitiesBuffer(64))
                .Add(new BodyContactDamage(50))
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DealDamageContactSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem());

            CurrentEnergy currentEnergy = entity.Get<CurrentEnergy>();
            TeleportEnergyCost teleportCost = entity.Get<TeleportEnergyCost>();
            IsDead isDead = entity.Get<IsDead>();
            CurrentHealth currentHealth = entity.Get<CurrentHealth>();
            InDeathProcess inDeathProcess = entity.Get<InDeathProcess>();
            
            CanSpendEnergy canSpendEnergy = new CanSpendEnergy();
            canSpendEnergy.Add(new FuncCondition(()=> !isDead.Value));
            
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
            
            entity
                .Add(canTeleport)
                .Add(canApplyDamage)
                .Add(mustDie)
                .Add(mustDieTriggers)
                .Add(mustSelfRelease)
                .Add(mustSelfReleaseTriggers)
                .Add(canSpendEnergy);
        }
    }
}