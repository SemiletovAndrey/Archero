using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.Sensers
{
    public class ContactDetectingMask
    {
        public readonly LayerMask Value;
        public ContactDetectingMask(LayerMask value) => Value = value;
    }

    public class ContactCollidersBuffer : Buffer<Collider>
    {
        public ContactCollidersBuffer(int initialSize) : base(initialSize)
        {
        }
    }

    public class ContactEntitiesBuffer : Buffer<Entity>
    {
        public ContactEntitiesBuffer(int initialSize) : base(initialSize)
        {
        }
    }

    public class DeathMask
    {
        public readonly LayerMask Value;
        public DeathMask(LayerMask value) => Value = value;
    }

    public class IsTouchDeathMask : ReactiveVariable<bool>
    {
        public IsTouchDeathMask()
        {
        }

        public IsTouchDeathMask(bool value) : base(value)
        {
        }
    }

    public class SplashHitCollidersBuffer : Buffer<Collider>
    {
        public SplashHitCollidersBuffer(int initialSize) : base(initialSize)
        {
        }
    }
}