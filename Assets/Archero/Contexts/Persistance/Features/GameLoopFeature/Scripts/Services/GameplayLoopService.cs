using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Persistance.Features.GameLoopFeature
{
    public class GameplayLoopService : ITickable
    {
        private readonly List<IGameUpdatable> _updatables;
        private bool _isRunning;

        public GameplayLoopService([InjectOptional] List<IGameUpdatable> updatables)
        {
            _updatables = updatables ?? new List<IGameUpdatable>();
        }

        public void Run() => _isRunning = true;
        public void Stop() => _isRunning = false;

        public void Tick()
        {
            if (!_isRunning)
                return;

            var dt = Time.deltaTime;
            for (var i = 0; i < _updatables.Count; i++)
                _updatables[i].Update(dt);
        }
    }
}