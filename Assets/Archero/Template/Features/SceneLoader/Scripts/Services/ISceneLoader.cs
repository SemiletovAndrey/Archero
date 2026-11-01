using System.Threading;
using Cysharp.Threading.Tasks;

namespace Template.Features.SceneLoader
{
    public interface ISceneLoader
    {
        public UniTask<LoadedScene> Load(string sceneName, CancellationToken ct);
        
        public UniTask<LoadedScene> Load<TPayload>(string sceneName, TPayload payload, CancellationToken ct)
            where TPayload : class;
        
        public UniTask Release(LoadedScene scene);

    }
}