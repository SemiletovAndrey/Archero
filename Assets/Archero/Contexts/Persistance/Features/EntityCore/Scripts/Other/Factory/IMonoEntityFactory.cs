using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public interface IMonoEntityFactory
    {
        public UniTask Preload(CancellationToken ct = default);
        public MonoEntity Create(Entity entity, Vector3 position, EntityType type);
    }
}