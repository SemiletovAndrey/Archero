using Cysharp.Threading.Tasks;

namespace Template.Features.DataStorage
{
    public interface IDataRepository
    {
        public UniTask<(bool,string)> TryRead(string key);
        public UniTask Write(string key, string serializedData);
        public UniTask Remove(string key);
        public UniTask<bool> Exists(string key);
    }
}