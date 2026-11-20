using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Energy
{
    public class EnergyRegenSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _current;
        private ReactiveVariable<float> _max;
        private ReactiveVariable<float> _regenPercent;
        private ReactiveVariable<float> _interval;
        private ReactiveVariable<float> _timer;

        public void OnInit(Entity entity)
        {
            _current = entity.Get<CurrentEnergy>();
            _max = entity.Get<MaxEnergy>();
            _regenPercent = entity.Get<EnergyRegenPercent>();
            _interval = entity.Get<EnergyRegenInterval>();
            _timer = entity.Get<EnergyRegenCurrentTime>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_interval.Value <= 0f)
                return;

            if (_current.Value >= _max.Value)
            {
                if (_timer.Value != 0f)
                    _timer.Value = 0f;
                return;
            }

            _timer.Value += deltaTime;
            
            while (_timer.Value >= _interval.Value && _current.Value < _max.Value)
            {
                _timer.Value -= _interval.Value;
                _current.Value = Mathf.Min(_max.Value, _current.Value + _max.Value * _regenPercent.Value);
            }
        }
    }
}