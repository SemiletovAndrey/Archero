using DataEditorFramework.Core.Data.Attribute;
using UnityEngine;

namespace DataEditorFramework.Example.AbstractTest
{
    [GameDataType]
    [CreateAssetMenu(fileName = "Aboba2", menuName = "Aboba2", order = 1)]
    public class AbobaImp2 :  AbstractAboba
    {
        [field: SerializeField] public string Text { get; private set; }
    }
}