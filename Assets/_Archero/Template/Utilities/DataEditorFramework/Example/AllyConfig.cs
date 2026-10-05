using System.Collections.Generic;
using DataEditorFramework.Core.Data.Attribute;
using UnityEngine;

namespace DataEditorFramework.Example
{    
    [CreateAssetMenu(fileName = "Ally", menuName = "AllyConfig")]
    [GameDataType]
    public class AllyConfig : ScriptableObject
    {
        public AllyType AllyType;
        public long Speed;
        public bool Aboba;
        public Vector3 Position;
        public GameObject Obj;
        
        [field: SerializeField] private List<EnemyConfig>  Enemies;
    }

    public enum AllyType
    {
        A,
        B,
        C
    }
}