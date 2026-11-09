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
        private IEntitiesFactory _entitiesFactory;
        private IInputBus _bus;
        
        private bool _isRunning;
        private Entity _testEntity;
        
        private Vector2 _currentInput;
        
        private MoveDirection _currentMoveDirection;

        [Inject]
        private void Construct(IEntitiesFactory entitiesFactory, IInputBus bus)
        {
            _entitiesFactory = entitiesFactory;
            _bus = bus;
        }

        public void Run()
        {
            _testEntity = _entitiesFactory.CreateTestEntity(Vector3.zero);
            
            _isRunning = true;
            
            _bus.Subscribe<MovementSignal>(OnMovementReceived);
            _currentMoveDirection = _testEntity.MoveDirectionC;
        }

        private void Update()
        {
            if (!_isRunning)
                return;
            
            Vector3 input = new Vector3(_currentInput.x, 0, _currentInput.y);
            _currentMoveDirection.Value.Value = input;
        }
        
        private void OnMovementReceived(MovementSignal signal)
        {
            _currentInput = signal.Value;
        }
    }
}