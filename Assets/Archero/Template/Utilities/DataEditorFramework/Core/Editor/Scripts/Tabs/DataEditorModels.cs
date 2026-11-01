#if  UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal readonly struct FieldDescriptor : IEquatable<FieldDescriptor>
    {
        public readonly string Key;
        public readonly string DisplayName;
        public readonly FieldInfo Field;

        public FieldDescriptor(string key, string displayName, FieldInfo field)
        {
            Key = key;
            DisplayName = displayName;
            Field = field;
        }

        public bool Equals(FieldDescriptor other)
        {
            return Key == other.Key && DisplayName == other.DisplayName && Equals(Field, other.Field);
        }

        public override bool Equals(object obj)
        {
            return obj is FieldDescriptor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, DisplayName, Field);
        }
    }

    internal sealed class FilterCondition
    {
        public string FieldKey;
        public FilterOperator Operator = FilterOperator.Equal;
        public VisualElement Row;
        public ToolbarMenu FieldMenu;
        public ToolbarMenu OperatorMenu;
        public VisualElement ValueHost;
        public ScriptableObject ValueCarrier;
        public string ActiveValuePropertyPath;
    }

    internal enum FilterEffectMode
    {
        Hide,
        Paint
    }

    internal enum FilterOperator
    {
        Less,
        Greater,
        GreaterOrEqual,
        LessOrEqual,
        Equal,
        NotEqual
    }
}
#endif