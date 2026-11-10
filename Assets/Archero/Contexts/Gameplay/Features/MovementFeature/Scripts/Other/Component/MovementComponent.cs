using Contexts.Scripts.Utilities.Reactive;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public class MoveDirection : ReactiveVariable<Vector3>
    {
        public MoveDirection() { }
        public MoveDirection(Vector3 value) : base(value) { }
    }

    public class MoveSpeed : ReactiveVariable<float>
    {
        public MoveSpeed() { }
        public MoveSpeed(float value) : base(value) { }
    }
}