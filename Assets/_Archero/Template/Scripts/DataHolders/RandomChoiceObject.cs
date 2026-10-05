using System;
using UnityEngine;

namespace Template.Scripts.DataHolders
{
    [Serializable]
    public struct RandomChoiceObject<T>
    {
        [field: SerializeField] public T Value { get; private set; }
        [field: SerializeField] public float Chances { get; private set; }

        public RandomChoiceObject(T value, float chances)
        {
            Value = value;
            Chances = chances;
        }
    }
}
