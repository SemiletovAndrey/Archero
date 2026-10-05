using DataEditorFramework.Core.Data.Attribute;
using UnityEngine;

namespace DataEditorFramework.Example.AbstractTest
{
    [GameDataType]
    [CreateAssetMenu(fileName = "Aboba1", menuName = "Aboba1", order = 0)]
    public class AbobaImp1 : AbstractAboba
    {
        [field: SerializeField] public int Stat { get; private set; }
    }
}