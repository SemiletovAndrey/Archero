using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.Sensers
{
    public class BodyContactsDetectingSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private Buffer<Collider> _contacts;
        private LayerMask _layerMask;

        private CapsuleCollider _body;


        public void OnInit(Entity entity)
        {
            _contacts = entity.Get<ContactCollidersBuffer>();
            _layerMask = entity.Get<ContactDetectingMask>().Value;

            _body = entity.Get<BodyCollider>().Collider;
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            _contacts.Count = Physics.OverlapCapsuleNonAlloc(_body.bounds.min, _body.bounds.max, _body.radius,
                _contacts.Items, _layerMask, QueryTriggerInteraction.Ignore);
            
            RemoveSelfFromContacts();
            
            Debug.Log($"Contacts detected: {_contacts.Count}");
        }

        private void RemoveSelfFromContacts()
        {
            int indexToRemove = -1;

            for (int i = 0; i < _contacts.Count; i++)
            {
                if (_contacts.Items[i] == _body)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove >= 0)
            {
                for (int i = indexToRemove; i < _contacts.Count - 1; i++)
                {
                    _contacts.Items[i] = _contacts.Items[i + 1];
                }

                _contacts.Count--;
            }
        }
    }
}