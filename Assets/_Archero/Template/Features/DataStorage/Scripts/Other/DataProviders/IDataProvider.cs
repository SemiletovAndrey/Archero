using Cysharp.Threading.Tasks;

namespace Template.Features.DataStorage
{
    public interface IDataProvider
    {
        public UniTask<bool> TryLoad();
        public UniTask Save();
        public UniTask Reset();
        public UniTask<bool> Exists();
    }

    public interface IDataProvider<out TData> : IDataProvider where TData : ISaveData
    {
        public TData Data { get; }
    }
}