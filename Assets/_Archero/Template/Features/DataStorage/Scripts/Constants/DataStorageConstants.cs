using UnityEngine;

namespace Template.Features.DataStorage
{
    public static class DataStorageConstants
    {
        public static string FolderPath = Application.isEditor ? Application.dataPath : Application.persistentDataPath;
        public static string SaveFileExtension = "json";
    }
}