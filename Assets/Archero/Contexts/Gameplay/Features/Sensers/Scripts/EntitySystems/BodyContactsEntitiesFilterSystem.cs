using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.Sensers
{
    public class BodyContactsEntitiesFilterSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private Buffer<Collider> _contacts;
        private Buffer<Entity> _contactsEntities;
        
        private readonly IColliderRegistryService _colliderRegistryService;

        public BodyContactsEntitiesFilterSystem(IColliderRegistryService colliderRegistryService)
        {
            _colliderRegistryService = colliderRegistryService;
        }

        public void OnInit(Entity entity)
        {
            _contacts = entity.Get<ContactCollidersBuffer>();
            _contactsEntities = entity.Get<ContactEntitiesBuffer>();
        }


        public void OnFixedUpdate(float fixedDeltaTime)
        {
            _contactsEntities.Count = 0;

            for (int i = 0; i < _contacts.Count; i++)
            {
                Collider collider = _contacts.Items[i];

                if (_colliderRegistryService.TryGetEntity(collider, out Entity contactEntity))
                {
                    _contactsEntities.Items[_contactsEntities.Count] = contactEntity;
                    _contactsEntities.Count += 1;
                }
            }
            
            Debug.Log($"Contacts entities: {_contacts.Count}");
        }
    }
}