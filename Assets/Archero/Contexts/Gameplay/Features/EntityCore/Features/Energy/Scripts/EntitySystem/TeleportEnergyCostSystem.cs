using System;
using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.EntityCore.Features.Energy
{
    public class TeleportEnergyCostSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _cost;
        private SpendEnergyRequest _spendRequest;

        private IDisposable _teleportDisposable;

        public void OnInit(Entity entity)
        {
            _cost = entity.Get<TeleportEnergyCost>();
            _spendRequest = entity.Get<SpendEnergyRequest>();
            
            _teleportDisposable = entity.Get<TeleportEvent>().Subscribe(OnTeleported);
        }

        public void OnDispose()
        {
            _teleportDisposable?.Dispose();
        }

        private void OnTeleported()
        {
            _spendRequest.Invoke(_cost.Value);
        }
    }
}