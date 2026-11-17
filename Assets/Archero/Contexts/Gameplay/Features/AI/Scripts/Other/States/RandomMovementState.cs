using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.GameplayStateMachine;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class RandomMovementState : State, IUpdatableState
    {
        private ReactiveVariable<Vector3> _movementDirection;
        private ReactiveVariable<Vector3> _rotationDirection;

        private float _coolDownBetweenDirectionGeneration;

        private float _time;

        public RandomMovementState(Entity entity, float coolDownBetweenDirectionGeneration)
        {
            _movementDirection = entity.Get<MoveDirection>();
            _rotationDirection = entity.Get<RotationDirection>();
            
            _coolDownBetweenDirectionGeneration = coolDownBetweenDirectionGeneration;
        }

        public override void Enter()
        {
            base.Enter();
            
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            _movementDirection.Value = randomDirection;
            _rotationDirection.Value = randomDirection;

            _time = 0;
        }

        public override void Exit()
        {
            base.Exit();
            
            _movementDirection.Value = Vector3.zero;
        }

        public void Update(float deltaTime)
        {
            _time += deltaTime;

            if (_time >= _coolDownBetweenDirectionGeneration)
            {
                GenerateNewDirection();
            }
        }

        private void GenerateNewDirection()
        {
            Vector3 inverseDirection = -_movementDirection.Value.normalized;
            Quaternion randomTurn = Quaternion.Euler(0, Random.Range(-30, 30), 0);
            Vector3 newDirection = randomTurn * inverseDirection;
            
            _movementDirection.Value = newDirection;
            _rotationDirection.Value = newDirection;
        }
    }
}