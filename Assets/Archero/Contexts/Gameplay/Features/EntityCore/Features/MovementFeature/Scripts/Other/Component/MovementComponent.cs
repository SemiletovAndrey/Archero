using System;
using Contexts.Scripts.Utilities.Reactive;
using Template.Scripts.Utils;
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
    
    public class RotationDirection : ReactiveVariable<Vector3>
    {
        public RotationDirection() { }
        public RotationDirection(Vector3 value) : base(value) { }
    }

    public class RotationSpeed : ReactiveVariable<float>
    {
        public RotationSpeed() { }
        public RotationSpeed(float value) : base(value) { }
    }

    public class CanMove : CompositeCondition
    {
        public CanMove() { }
        public CanMove(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }
    
    public class CanRotate : CompositeCondition
    {
        public CanRotate() { }
        public CanRotate(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }

    public class IsMoving : ReactiveVariable<bool>
    {
        public IsMoving() { }
        public IsMoving(bool value) : base(value) { }
    }
    
    public class TeleportRadius : ReactiveVariable<float>
    {
        public TeleportRadius() { }
        public TeleportRadius(float value) : base(value) { }
    }
    
    public class TeleportRequest : ReactiveEvent<Vector3> { }
    public class TeleportEvent : ReactiveEvent { }
    
    public class CanTeleport : CompositeCondition
    {
        public CanTeleport() { }
        public CanTeleport(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }
}