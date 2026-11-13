using Template.Features.NewInputFeature;

namespace Gameplay.Features.LifeCycle
{
    public readonly struct AttackSignal : IInputSignal
    {
        public readonly bool IsPressed;

        public AttackSignal(bool isPressed)
        {
            IsPressed = isPressed;
        }
    }
}