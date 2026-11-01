using System;
using System.Collections;
using System.Reflection;

namespace DataEditorFramework.Core.Data
{
    internal abstract class FieldValueSortRule : IDataSortRule
    {
        public abstract bool CanSort(FieldInfo field);

        public int Compare(object leftItem, object rightItem, FieldInfo field)
        {
            return DataSortValueComparer.Compare(GetValue(leftItem, field), GetValue(rightItem, field));
        }

        protected static object GetValue(object item, FieldInfo field)
        {
            if (item == null)
                return null;

            try
            {
                return field.GetValue(item);
            }
            catch
            {
                return null;
            }
        }
    }

    internal sealed class ComparableDataSortRule : FieldValueSortRule
    {
        public override bool CanSort(FieldInfo field)
        {
            var fieldType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
            return fieldType == typeof(string)
                   || fieldType.IsPrimitive
                   || fieldType.IsEnum
                   || fieldType == typeof(decimal)
                   || fieldType == typeof(DateTime)
                   || fieldType == typeof(TimeSpan)
                   || fieldType == typeof(Guid);
        }
    }

    internal sealed class UnityObjectDataSortRule : IDataSortRule
    {
        public bool CanSort(FieldInfo field)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType);
        }

        public int Compare(object leftItem, object rightItem, FieldInfo field)
        {
            return DataSortValueComparer.Compare(GetObjectName(leftItem, field), GetObjectName(rightItem, field));
        }

        private static string GetObjectName(object item, FieldInfo field)
        {
            try
            {
                return field.GetValue(item) is UnityEngine.Object unityObject && unityObject
                    ? unityObject.name
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }

    internal sealed class CollectionCountDataSortRule : IDataSortRule
    {
        public bool CanSort(FieldInfo field)
        {
            return field.FieldType.IsArray || typeof(IList).IsAssignableFrom(field.FieldType);
        }

        public int Compare(object leftItem, object rightItem, FieldInfo field)
        {
            return DataSortValueComparer.Compare(GetCount(leftItem, field), GetCount(rightItem, field));
        }

        private static int? GetCount(object item, FieldInfo field)
        {
            try
            {
                return field.GetValue(item) is IList list ? list.Count : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
