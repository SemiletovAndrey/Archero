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

    public interface IDataProvider<TData> : IDataProvider where TData : ISaveData
    {
        public TData Data { get; }
        
        public void RegisterWriter(IDataWriter<TData> writer);
        public void RegisterReader(IDataReader<TData> reader);
    }
}