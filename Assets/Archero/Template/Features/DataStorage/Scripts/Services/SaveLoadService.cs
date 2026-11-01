using Cysharp.Threading.Tasks;

namespace Template.Features.DataStorage
{
    public class SaveLoadService : ISaveLoadService
    {
        private readonly IDataSerializer _serializer;
        private readonly IDataKeyStorage _dataKeyStorage;
        private readonly IDataRepository _dataRepository;

        public SaveLoadService(IDataSerializer serializer,
            IDataKeyStorage dataKeyStorage,
            IDataRepository dataRepository)
        {
            _serializer = serializer;
            _dataKeyStorage = dataKeyStorage;
            _dataRepository = dataRepository;
        }

        public async UniTask<(bool, TData)> TryLoad<TData>() where TData : ISaveData
        {
            string key = _dataKeyStorage.GetKeyFor<TData>();

            var (success, serializedData) = await _dataRepository.TryRead(key);

            TData data = _serializer.Deserialize<TData>(serializedData);
            if (success)
            {
                return ((true, data));
            }

            return ((false, data));
        }

        public async UniTask<T> LoadOrDefault<T>(bool saveIfDefault = true) where T : ISaveData
        {
            string key = _dataKeyStorage.GetKeyFor<T>();
            var (sucess, serializedData) = await _dataRepository.TryRead(key);
            if (sucess)
            {
                return _serializer.Deserialize<T>(serializedData);
            }

            T model = default(T);

            if (saveIfDefault)
            {
                await Save(model);
            }

            return model;
        }

        public async UniTask Save<TData>(TData data) where TData : ISaveData
        {
            string serializeData = _serializer.Serialize(data);
            string key = _dataKeyStorage.GetKeyFor<TData>();
            await _dataRepository.Write(key, serializeData);
        }

        public async UniTask Remove<TData>() where TData : ISaveData
        {
            string key = _dataKeyStorage.GetKeyFor<TData>();
            await _dataRepository.Remove(key);
        }

        public async UniTask<bool> Exists<TData>() where TData : ISaveData
        {
            string key = _dataKeyStorage.GetKeyFor<TData>();
            return await _dataRepository.Exists(key);
        }
    }
}