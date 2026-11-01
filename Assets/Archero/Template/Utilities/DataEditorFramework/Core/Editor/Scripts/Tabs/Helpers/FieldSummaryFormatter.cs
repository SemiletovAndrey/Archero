using System.Collections;
using System.Reflection;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal static class FieldSummaryFormatter
    {
        public static bool ShouldUseSummary(FieldInfo field)
        {
            var fieldType = field.FieldType;
            if (fieldType == typeof(string))
                return false;

            if (fieldType.IsArray)
                return true;

            if (typeof(IList).IsAssignableFrom(fieldType))
                return true;

            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
                return false;

            if (fieldType.IsPrimitive || fieldType.IsEnum)
                return false;

            if (fieldType == typeof(decimal))
                return false;

            return !fieldType.IsValueType;
        }

        public static string BuildText(object item, FieldInfo field)
        {
            object value;
            try
            {
                value = field.GetValue(item);
            }
            catch
            {
                return "Unavailable";
            }

            if (value == null)
            {
                if (field.FieldType.IsArray || typeof(IList).IsAssignableFrom(field.FieldType))
                    return "Count: 0";

                return "Null";
            }

            if (value is string s)
                return s;

            if (value is IList list)
                return $"Count: {list.Count}";

            if (value is UnityEngine.Object unityObject)
                return unityObject ? unityObject.name : "Null";

            return value.ToString();
        }
    }
}
