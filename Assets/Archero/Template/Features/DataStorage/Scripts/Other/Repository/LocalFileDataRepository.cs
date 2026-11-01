using System.IO;
using Cysharp.Threading.Tasks;

namespace Template.Features.DataStorage
{
    public class LocalFileDataRepository : IDataRepository
    {
        private readonly string _folderPath;
        private readonly string _saveFileExtension;

        public LocalFileDataRepository(string folderPath, string saveFileExtension)
        {
            _folderPath = folderPath;
            _saveFileExtension = saveFileExtension;
        }

        public UniTask<(bool, string)> TryRead(string key)
        {
            if(File.Exists(FullPathFor(key)) == false)
                return UniTask.FromResult((false, string.Empty));
            
            return UniTask.FromResult((true, File.ReadAllText(FullPathFor(key))));
        }

        public UniTask Write(string key, string serializedData)
        {
            File.WriteAllText(FullPathFor(key), serializedData);
            return UniTask.CompletedTask; 
        }

        public UniTask Remove(string key)
        {
            File.Delete(FullPathFor(key));
            return UniTask.CompletedTask;
        }

        public UniTask<bool> Exists(string key)
        {
            bool exists = File.Exists(FullPathFor(key));
            return UniTask.FromResult(exists);
        }

        private string FullPathFor(string key)
        {
            return Path.Combine(_folderPath, key) + "." + _saveFileExtension;
        }
    }
}