using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Energy
{
    public class SpendEnergySystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _current;
        private CanSpendEnergy _canSpend;
        private SpendEnergyEvent _spendEvent;

        private IDisposable _requestDisposable;

        public void OnInit(Entity entity)
        {
            _current = entity.Get<CurrentEnergy>();
            _canSpend = entity.Get<CanSpendEnergy>();
            _spendEvent = entity.Get<SpendEnergyEvent>();

            _requestDisposable = entity.Get<SpendEnergyRequest>().Subscribe(OnRequest);
        }

        public void OnDispose()
        {
            _requestDisposable?.Dispose();
        }

        private void OnRequest(float amount)
        {
            if (amount <= 0f)
                return;

            if (!_canSpend.Evaluate())
                return;

            if (_current.Value < amount)
                return;

            _current.Value -= amount;
            _spendEvent.Invoke(amount);
        }
    }
}