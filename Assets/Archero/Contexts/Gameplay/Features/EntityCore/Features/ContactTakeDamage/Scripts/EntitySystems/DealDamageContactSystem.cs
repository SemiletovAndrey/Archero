using System.Collections.Generic;
using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;

namespace Gameplay.Features.ContactTakeDamage
{
    public class DealDamageContactSystem : IInitializableSystem, IUpdatableSystem
    {
        private Buffer<Entity> _contacts;
        private ReactiveVariable<float> _damage;

        private List<Entity> _processEntities;

        public void OnInit(Entity entity)
        {
            _contacts = entity.Get<ContactEntitiesBuffer>();
            _damage = entity.Get<BodyContactDamage>();

            _processEntities = new List<Entity>(_contacts.Items.Length);
        }

        public void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                Entity contactEntity = _contacts.Items[i];
                if (_processEntities.Contains(contactEntity) == false)
                {
                    _processEntities.Add(contactEntity);

                    if (contactEntity.Has<TakeDamageRequest>())
                    {
                        contactEntity.Get<TakeDamageRequest>().Invoke(_damage.Value);
                    }
                }
            }

            for (int i = _processEntities.Count - 1; i >= 0; i--)
            {
                if (ContainInContacts(_processEntities[i]) == false)
                {
                    _processEntities.RemoveAt(i);
                }
            }
        }
        
        private bool ContainInContacts(Entity contactEntity)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                if(_contacts.Items[i] == contactEntity)
                    return true;
            }
            return false;
        }
    }
}