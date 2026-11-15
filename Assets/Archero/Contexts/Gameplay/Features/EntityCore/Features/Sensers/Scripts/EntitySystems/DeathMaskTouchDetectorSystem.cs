using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.Sensers
{
    public class DeathMaskTouchDetectorSystem : IInitializableSystem, IUpdatableSystem
    {
        private Buffer<Collider> _contacts;
        private ReactiveVariable<bool> _isTouchDeathMask;
        private LayerMask _deathMask;
        
        public void OnInit(Entity entity)
        {
            _contacts = entity.Get<ContactCollidersBuffer>();
            _isTouchDeathMask = entity.Get<IsTouchDeathMask>();
            _deathMask = entity.Get<DeathMask>().Value;
        }

        public void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                if (MatchWithDeathLayer(_contacts.Items[i]))
                {
                    _isTouchDeathMask.Value = true;
                    return;
                }
            }
            
            _isTouchDeathMask.Value = false;
        }

        private bool MatchWithDeathLayer(Collider collider)
        {
            return ((1 << collider.gameObject.layer) & _deathMask) != 0;
        }
    }
}