using System;
using System.Reflection;

namespace DataEditorFramework.Core.Editor.Scripts.Extensions
{
    public static class FieldInfoExtensions
    {
        public static string GetDisplayName(this FieldInfo field)
        {
            var fieldName = field.Name;
            const string backingFieldSuffix = ">k__BackingField";

            if (!fieldName.StartsWith("<", StringComparison.Ordinal) ||
                !fieldName.EndsWith(backingFieldSuffix, StringComparison.Ordinal))
                return fieldName;

            var trimmedName = fieldName.Substring(1, fieldName.Length - backingFieldSuffix.Length - 1);

            return trimmedName;
        }
    }
}