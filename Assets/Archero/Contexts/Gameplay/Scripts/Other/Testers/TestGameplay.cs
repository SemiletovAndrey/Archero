using System;
using Gameplay.Features.LifeCycle;
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
        private RotationDirection _currentRotationDirection;
        private CurrentHealth _currentHealth;

        [Inject]
        private void Construct(IEntityFactory entityFactory, IInputBus bus)
        {
            _entityFactory = entityFactory;
            _bus = bus;
        }

        public void Run()
        {
            _testEntity = _entityFactory.Create(EntityType.Ghost, Vector3.zero);
            _currentMoveDirection = _testEntity.Get<MoveDirection>();
            _currentRotationDirection = _testEntity.Get<RotationDirection>();
            _currentHealth = _testEntity.Get<CurrentHealth>();

            _bus.Subscribe<MovementSignal>(OnMovementReceived);
            _bus.Subscribe<AttackSignal>(OnAttackReceived);
            _isRunning = true;
        }

        private void OnDestroy()
        {
            _bus.Unsubscribe<MovementSignal>(OnMovementReceived);
            _bus.Unsubscribe<AttackSignal>(OnAttackReceived);
        }

        private void Update()
        {
            if (!_isRunning)
                return;

            Vector3 input = new Vector3(_currentInput.x, 0, _currentInput.y);
            _currentMoveDirection.Value = input;
            _currentRotationDirection.Value = input;
        }

        private void OnMovementReceived(MovementSignal signal)
        {
            _currentInput = signal.Value;
        }

        private void OnAttackReceived(AttackSignal signal)
        {
            if (signal.IsPressed)
            {
                _currentHealth.Value -= 50;
                Debug.Log($"Current health {_currentHealth.Value}");
            }
        }
    }
}