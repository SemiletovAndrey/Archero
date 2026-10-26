using Cysharp.Threading.Tasks;

namespace Template.Features.DataStorage
{
    public interface ISaveLoadService
    {
        public UniTask<(bool, TData)> TryLoad<TData>() where TData : ISaveData;
        public UniTask<T> LoadOrDefault<T>(bool saveIfDefault = true) where T : ISaveData;
        public UniTask Save<TData>(TData data) where TData : ISaveData;
        public UniTask Remove<TData>() where TData : ISaveData;
        public UniTask<bool> Exists<TData>() where TData : ISaveData;
    }
}