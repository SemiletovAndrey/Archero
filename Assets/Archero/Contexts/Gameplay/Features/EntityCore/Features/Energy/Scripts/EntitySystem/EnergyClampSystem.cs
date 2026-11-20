using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Energy
{
    public class EnergyClampSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _current;
        private ReactiveVariable<float> _max;

        private IDisposable _currentDisposable;
        private IDisposable _maxDisposable;

        public void OnInit(Entity entity)
        {
            _current = entity.Get<CurrentEnergy>();
            _max = entity.Get<MaxEnergy>();

            _currentDisposable = _current.Subscribe(OnChanged);
            _maxDisposable = _max.Subscribe(OnChanged);

            Clamp();
        }

        public void OnDispose()
        {
            _currentDisposable?.Dispose();
            _maxDisposable?.Dispose();
        }

        private void OnChanged(float oldValue, float newValue) => Clamp();

        private void Clamp()
        {
            float clamped = Mathf.Clamp(_current.Value, 0f, Mathf.Max(0f, _max.Value));

            if (!Mathf.Approximately(clamped, _current.Value))
                _current.Value = clamped;
        }
    }
}