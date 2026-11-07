using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using UnityEngine;
using Zenject;

namespace Gameplay.Scripts
{
    public class TestGameplay : MonoBehaviour
    {
        private IEntitiesFactory _entitiesFactory;
        
        private bool _isRunning;

        [Inject]
        private void Construct(IEntitiesFactory entitiesFactory)
        {
            _entitiesFactory = entitiesFactory;
        }
        
        public void Run()
        {
            Entity testEntity = _entitiesFactory.CreateTestEntity(Vector3.zero);
            
            Debug.Log($"Direction {testEntity.GetComponent<MoveDirection>().Value.Value};");
            Debug.Log($"Speed {testEntity.GetComponent<MoveSpeed>().Value.Value};");
            
            _isRunning = true;
        }

        private void Update()
        {
            if (!_isRunning)
                return;
        }
    }
}