using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Domain.AddressableLoading
{
    public record AddressableLoadResult(GameObject Prefab, AsyncOperationHandle Handle);
}