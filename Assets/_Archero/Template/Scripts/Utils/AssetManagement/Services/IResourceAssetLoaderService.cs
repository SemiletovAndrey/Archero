using UnityEngine;

namespace Utilities.AssetManagement
{
    public interface IResourceAssetLoaderService
    {
        public T Load<T>(string path) where T : Object;
    }
}