using System;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using Template.Features.NewInputFeature;
using Template.Features.NewInputFeature.Scripts.Example;
using UnityEngine;
using Zenject;

namespace Gameplay.Scripts
{
    public class TestGameplay : MonoBehaviour
    {
        private IEntityFactory _entityFactory;
        private IInputBus _bus;

        private bool _isRunning;
        private Entity _testEntity;
        private Vector2 _currentInput;
        private MoveDirection _currentMoveDirection;

        [Inject]
        private void Construct(IEntityFactory entityFactory, IInputBus bus)
        {
            _entityFactory = entityFactory;
            _bus = bus;
        }

        public void Run()
        {
            _testEntity = _entityFactory.Create(EntityType.Test, Vector3.zero);
            _currentMoveDirection = _testEntity.Get<MoveDirection>();

            _bus.Subscribe<MovementSignal>(OnMovementReceived);
            _isRunning = true;
        }

        private void Update()
        {
            if (!_isRunning)
                return;

            _currentMoveDirection.Value = new Vector3(_currentInput.x, 0, _currentInput.y);
        }

        private void OnMovementReceived(MovementSignal signal)
        {
            _currentInput = signal.Value;
        } 
    }
}