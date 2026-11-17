using System;
using System.Threading;
using Contexts.Scripts.Utilities.Reactive;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Archero.Template.Scripts.Utils
{
    public class TimerService : IDisposable
    {
        private float _coolDown;

        private ReactiveEvent _coolDownEnded;
        private ReactiveVariable<float> _currentTime;
        
        private CancellationTokenSource _cts;

        public TimerService(float coolDown)
        {
            _coolDown = coolDown;
            _currentTime = new ReactiveVariable<float>();
            _coolDownEnded = new ReactiveEvent();
        }
        
        public IReadOnlyEvent CoolDownEnded => _coolDownEnded;
        public IReadOnlyVariable<float>  CurrentTime => _currentTime;
        public bool IsOver => _currentTime.Value <= 0;

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            if (_cts == null)
                return;

            if (!_cts.IsCancellationRequested)
                _cts.Cancel();

            _cts.Dispose();
            _cts = null;
        }

        public void Restart()
        {
            Stop();

            _cts = new CancellationTokenSource();
            CoolDownProcess(_cts.Token).Forget();
        }

        private async UniTaskVoid CoolDownProcess(CancellationToken cancellationToken = default)
        {
            _currentTime.Value = _coolDown;

            while (_currentTime.Value > 0f)
            {
                bool isCanceled = await UniTask
                    .Yield(PlayerLoopTiming.Update, cancellationToken)
                    .SuppressCancellationThrow();

                if (isCanceled)
                    return;

                _currentTime.Value -= Time.deltaTime;
            }

            _currentTime.Value = 0f;
            _coolDownEnded?.Invoke();
        } 
    }
}