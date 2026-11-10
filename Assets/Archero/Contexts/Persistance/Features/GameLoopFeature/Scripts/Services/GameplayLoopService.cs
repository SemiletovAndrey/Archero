using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Persistance.Features.GameLoopFeature
{
    public class GameplayLoopService : ITickable, IFixedTickable
    {
        private readonly List<IGameUpdatable> _updatables;
        private readonly List<IGameFixedUpdatable> _fixedUpdatables;
        private bool _isRunning;

        public GameplayLoopService([InjectOptional] List<IGameUpdatable> updatables, [InjectOptional] List<IGameFixedUpdatable> fixedUpdatables)
        {
            _updatables = updatables ?? new List<IGameUpdatable>();
            _fixedUpdatables = fixedUpdatables ?? new List<IGameFixedUpdatable>();
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

        public void FixedTick()
        {
            if (!_isRunning)
                return;
 
            var dt = Time.fixedDeltaTime;
            for (var i = 0; i < _fixedUpdatables.Count; i++)
                _fixedUpdatables[i].FixedUpdate(dt);
        }
    }
}