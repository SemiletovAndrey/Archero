using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public class MoveDirection : IEntityComponent
    {
        public ReactiveVariable<Vector3> Value;
    }

    public class MoveSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }


}