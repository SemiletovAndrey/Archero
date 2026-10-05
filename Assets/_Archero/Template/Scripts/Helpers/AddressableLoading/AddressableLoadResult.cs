using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RivalDom.Scripts.Domain.AddressableLoading
{
    public record AddressableLoadResult(GameObject Prefab, AsyncOperationHandle Handle);
}