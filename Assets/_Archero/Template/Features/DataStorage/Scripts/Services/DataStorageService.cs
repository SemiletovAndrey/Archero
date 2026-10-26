using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Template.Scripts.Attributes;
using UnityEngine;

namespace Template.Features.DataStorage
{
    [Obsolete("Use SaveLoadService instead.")]
    public class DataStorageService : IDataStorageService
    {
        public void Init()
        {
        }

        public void Save(BaseUserData userData)
        {
            string key = GetDefaultKey(userData.GetType());
            string serializedData = JsonConvert.SerializeObject(userData);
            
            PlayerPrefs.SetString(key, serializedData);
        }

        public bool TryLoad<T>(out T userData) where T : BaseUserData
        {
            if (!DataExists<T>(out string key))
            {
                userData = null;
                return false;
            }
            
            string serializedData = PlayerPrefs.GetString(key);
            userData = JsonConvert.DeserializeObject<T>(serializedData);
            
            return true;
        }

        public T LoadOrDefault<T>(bool saveIfDefault = true) where T : BaseUserData, new()
        {
            if (!DataExists<T>(out string key))
            {
                T model = new();
                
                if (saveIfDefault)
                {
                    Save(model);
                }
                
                return model;
            }
            
            string serializedData = PlayerPrefs.GetString(key);
            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        public T LoadOrDefault<T>(T defaultValue, bool saveIfDefault) where T : BaseUserData
        {
            if (!DataExists<T>(out string key))
            {
                T data = defaultValue;
                
                if (saveIfDefault)
                {
                    Save(data);
                }
                
                return data;
            }
            
            string serializedData = PlayerPrefs.GetString(key);
            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        public T LoadOrCreate<T>(Func<T> creator, bool saveAfterCreate) where T : BaseUserData
        {
            if (!DataExists<T>(out string key))
            {
                T data = creator();
                
                if (saveAfterCreate)
                {
                    Save(data);
                }

                return data;
            }
            
            string serializedData = PlayerPrefs.GetString(key);
            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        private string GetDefaultKey<T>() where T : BaseUserData
        {
            return GetDefaultKey(typeof(T));
        }
        
        private string GetDefaultKey(Type dataType)
        {
            return dataType.Name;
        }

        private bool DataExists<T>(out string key) where T : BaseUserData
        {
            return DataExists(typeof(T), out key);
        }
        
        private bool DataExists(Type dataType, out string key)
        {
            string defaultKey = GetDefaultKey(dataType);
            if (KeyExists(defaultKey))
            {
                key = defaultKey;
                return true;
            }

            IEnumerable<FormerNameAttribute> formerNames = dataType.GetCustomAttributes<FormerNameAttribute>();
            foreach (var formerName in formerNames)
            {
                if (KeyExists(formerName.Value))
                {
                    key = formerName.Value;
                    return true;
                }
            }

            key = null;
            return false;
        }

        private bool KeyExists(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}
