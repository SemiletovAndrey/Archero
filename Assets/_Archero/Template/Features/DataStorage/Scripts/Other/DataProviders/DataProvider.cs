using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Localization.Plugins.XLIFF.V20;

namespace Template.Features.DataStorage
{
    public abstract class DataProvider<TData> : IDataProvider<TData> where TData : ISaveData
    {
        private readonly ISaveLoadService _saveLoadService;
        
        private readonly List<IDataWriter<TData>> _writers = new();
        private readonly List<IDataReader<TData>> _readers = new();
        
        public TData Data { get; private set; }

        protected DataProvider(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public void RegisterWriter(IDataWriter<TData> writer)
        {
            if (_writers.Contains(writer))
                throw new ArgumentException(nameof(writer));
            
            _writers.Add(writer);
        }
        
        public void RegisterReader(IDataReader<TData> reader)
        {
            if (_readers.Contains(reader))
                throw new ArgumentException(nameof(reader));
            
            _readers.Add(reader);
        }

        public async UniTask<bool> TryLoad()
        {
            var (success, data) = await _saveLoadService.TryLoad<TData>();
            Data = success ? data : GetOriginData();

            SendDataToReaders();
            
            return success;
        }
        
        public async UniTask<bool> Exists()
        {
            return await _saveLoadService.Exists<TData>();
        }
        
        public async UniTask Save()
        {
            UpdateDataToWriters();
            
            await _saveLoadService.Save(Data);
        }

        public async UniTask Reset()
        {
            Data = GetOriginData();
            
            SendDataToReaders();
            
            await Save();
        }

        protected abstract TData GetOriginData();
        
        private void SendDataToReaders()
        {
            foreach (IDataReader<TData> reader in _readers)
            {
                reader.ReadFrom(Data);
            }
        }
        
        private void UpdateDataToWriters()
        {
            foreach (IDataWriter<TData> writer in _writers)
            {
                writer.WriteTo(Data);
            }
        }
    }
}