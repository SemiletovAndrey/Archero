using DataEditorFramework.Core.Data.Attribute;
using UnityEngine;

namespace DataEditorFramework.Example
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "EnemyConfig")]
    [GameDataType]
    public class EnemyConfig : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; }
        public AnimationCurve Aboooba { get; private set; }
        public int Speed;
        public bool Aboba;
        public GameObject Obj;
    }
}