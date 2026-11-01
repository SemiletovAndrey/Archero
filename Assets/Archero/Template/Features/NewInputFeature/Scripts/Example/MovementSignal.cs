using UnityEngine;

namespace Template.Features.NewInputFeature.Scripts.Example
{
    public struct MovementSignal : IInputSignal
    {
        public readonly Vector2 Value;
        public MovementSignal(Vector2 value) => Value = value;
    }
}