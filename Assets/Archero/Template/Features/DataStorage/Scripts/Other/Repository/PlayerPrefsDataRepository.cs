using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.DataStorage
{
    public class PlayerPrefsDataRepository : IDataRepository
    {
        public UniTask<(bool, string)> TryRead(string key)
        {
            if (PlayerPrefs.HasKey(key) == false)
                return UniTask.FromResult((false, string.Empty));
            string text = PlayerPrefs.GetString(key);
            return UniTask.FromResult((true, text));
        }

        public UniTask Write(string key, string serializedData)
        {
            PlayerPrefs.SetString(key, serializedData);
            return UniTask.CompletedTask;
        }

        public UniTask Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);
            return UniTask.CompletedTask;
        }

        public UniTask<bool> Exists(string key)
        {
            bool hasKey = PlayerPrefs.HasKey(key);
            return UniTask.FromResult(hasKey);
        }
    }
}