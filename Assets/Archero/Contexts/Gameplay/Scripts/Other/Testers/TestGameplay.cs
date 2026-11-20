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
        private IInputBus _inputBus;

        private bool _isRunning;
        private Entity _testEntity;
        private Entity _ghost;
        private Entity _teleportator;

        [Inject]
        private void Construct(IEntityFactory entityFactory, IBrainsFactory brainsFactory, IInputBus inputBus)
        {
            _entityFactory = entityFactory;
            _brainsFactory = brainsFactory;
            _inputBus = inputBus;
        }

        public void Run()
        {
            _testEntity = _entityFactory.Create(EntityType.Hero, Vector3.zero - Vector3.forward * 10);
            _ghost = _entityFactory.Create(EntityType.Ghost, Vector3.zero + Vector3.forward * 5);
            _teleportator = _entityFactory.Create(EntityType.Teleportator, Vector3.zero + Vector3.forward * 2);
            _testEntity.Add(new CurrentTarget());
            _teleportator.Add(new CurrentTarget());

            _inputBus.Subscribe<AttackSignal>(OnAttackSignal);
            _isRunning = true;
        }

        private void OnDestroy()
        {
            _inputBus.Unsubscribe<AttackSignal>(OnAttackSignal);
        }

        private void Update()
        {
        }

        private void OnAttackSignal(AttackSignal signal)
        {
        }

        [ContextMenu("AddBrainFor Ghost")]
        private void AddBrainForGhost()
        {
            _brainsFactory.CreateGhostBrain(_ghost);
        }
        
        [ContextMenu("AddBrainFor Teleportator")]
        private void AddBrainForTeleportator()
        {
            _brainsFactory.CreateTeleportatorBrain(_teleportator);
        }
        
        [ContextMenu("AddBrainFor Main hero")]
        private void AddBrainForMainHero()
        {
            _brainsFactory.CreateMainHeroBrain(_testEntity, new NearestDamageableTargetSelector(_testEntity));
        }
    }
}