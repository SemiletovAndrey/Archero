using UnityEngine;

namespace Utilities.AssetManagement
{
    public class ResourceAssetLoaderService : IResourceAssetLoaderService
    {
        public T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }
}