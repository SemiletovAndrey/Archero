using System;
using System.Collections.Generic;
using Persistance.Features.User.Features.JsonUser;
using Template.Features.Localization;

namespace Template.Features.DataStorage
{
    public class MapDataKeyStorage : IDataKeyStorage
    {
        private readonly Dictionary<Type, string> _keys = new Dictionary<Type, string>()
        {
            { typeof(PlayerData), typeof(PlayerData).ToString() },
            { typeof(LocalizationUserData), typeof(LocalizationUserData).ToString() },
        };

        public string GetKeyFor<TData>() where TData : ISaveData
        {
            return _keys[typeof(TData)];
        }
    }
}