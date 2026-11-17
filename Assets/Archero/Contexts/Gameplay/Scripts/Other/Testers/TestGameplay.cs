using Gameplay.Features.AI;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.EntityCore.Features.Attack;
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
        private IBrainsFactory _brainsFactory;

        private bool _isRunning;
        private Entity _testEntity;
        private Entity _ghost;
        
        private Vector2 _currentInput;
        private MoveDirection _currentMoveDirection;
        private RotationDirection _currentRotationDirection;

        [Inject]
        private void Construct(IEntityFactory entityFactory, IInputBus bus, IBrainsFactory brainsFactory)
        {
            _entityFactory = entityFactory;
            _bus = bus;
            _brainsFactory = brainsFactory;
        }

        public void Run()
        {
            _testEntity = _entityFactory.Create(EntityType.Hero, Vector3.zero);
            _ghost = _entityFactory.Create(EntityType.Ghost, Vector3.zero + Vector3.forward * 5);
            
            _currentMoveDirection = _testEntity.Get<MoveDirection>();
            _currentRotationDirection = _testEntity.Get<RotationDirection>();

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
                _testEntity.Get<StartAttackRequest>().Invoke();
        }

        [ContextMenu("AddBrainFor Ghost")]
        private void AddBrainForGhost()
        {
            _brainsFactory.CreateGhostBrain(_ghost);
        }
    }
}