using System;

namespace Template.Features.DataStorage
{
    public interface IDataStorageService
    {
        public void Save(BaseUserData userData);

        public bool TryLoad<TData>(out TData userData) where TData : BaseUserData;
        public TData LoadOrDefault<TData>(bool saveIfDefault = true) where TData : BaseUserData, new();
        public TData LoadOrDefault<TData>(TData defaultValue, bool saveIfDefault = true) where TData : BaseUserData;
        public TData LoadOrCreate<TData>(Func<TData> creator, bool saveAfterCreate = true) where TData : BaseUserData;
    }
}