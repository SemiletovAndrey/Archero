using System;
using Contexts.Scripts.Utilities.Reactive;
using Template.Scripts.Utils;

namespace Gameplay.Features.EntityCore.Features.Energy
{
    public class CurrentEnergy : ReactiveVariable<float>
    {
        public CurrentEnergy() { }

        public CurrentEnergy(float value) : base(value) { }
    }
    
    public class MaxEnergy : ReactiveVariable<float>
    {
        public MaxEnergy() { }

        public MaxEnergy(float value) : base(value) { }
    }
    
    public class EnergyRegenPercent : ReactiveVariable<float>
    {
        public EnergyRegenPercent() { }
        public EnergyRegenPercent(float value) : base(value) { }
    }

    public class EnergyRegenInterval : ReactiveVariable<float>
    {
        public EnergyRegenInterval() { }
        public EnergyRegenInterval(float value) : base(value) { }
    }

    public class EnergyRegenCurrentTime : ReactiveVariable<float>
    {
        public EnergyRegenCurrentTime() { }
        public EnergyRegenCurrentTime(float value) : base(value) { }
    }

    public class TeleportEnergyCost : ReactiveVariable<float>
    {
        public TeleportEnergyCost() { }
        public TeleportEnergyCost(float value) : base(value) { }
    }
    
    public class SpendEnergyRequest : ReactiveEvent<float>
    {
    }

    public class SpendEnergyEvent : ReactiveEvent<float>
    {
    }

    public class CanSpendEnergy : CompositeCondition
    {
        public CanSpendEnergy() { }
        public CanSpendEnergy(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }
}