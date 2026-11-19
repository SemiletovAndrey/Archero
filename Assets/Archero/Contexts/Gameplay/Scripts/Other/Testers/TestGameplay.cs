using Gameplay.Features.AI;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using Template.Features.NewInputFeature;
using Template.Features.NewInputFeature.Scripts.Example;
using UnityEngine;
using Zenject;

namespace Gameplay.Scripts
{
    public class TestGameplay : MonoBehaviour
    {
        private IEntityFactory _entityFactory;
        private IBrainsFactory _brainsFactory;

        private bool _isRunning;
        private Entity _testEntity;
        private Entity _ghost;

        [Inject]
        private void Construct(IEntityFactory entityFactory, IBrainsFactory brainsFactory)
        {
            _entityFactory = entityFactory;
            _brainsFactory = brainsFactory;
        }

        public void Run()
        {
            _testEntity = _entityFactory.Create(EntityType.Hero, Vector3.zero);
            _ghost = _entityFactory.Create(EntityType.Ghost, Vector3.zero + Vector3.forward * 5);
            _testEntity.Add<CurrentTarget>(new CurrentTarget());
            _brainsFactory.CreateMainHeroBrain(_testEntity, new NearestDamageableTargetSelector(_testEntity));

            _isRunning = true;
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
        }

        [ContextMenu("AddBrainFor Ghost")]
        private void AddBrainForGhost()
        {
            _brainsFactory.CreateGhostBrain(_ghost);
        }
    }
}