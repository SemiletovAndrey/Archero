using DataEditorFramework.Core.Data.Attribute;
using UnityEngine;

namespace DataEditorFramework.Example.AbstractTest
{
    [GameDataType]
    public abstract class AbstractAboba : ScriptableObject
    {
        [field: SerializeField] public bool Aboba { get; private set; }
    }
}