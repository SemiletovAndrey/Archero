using System.Reflection;

namespace DataEditorFramework.Core.Data
{
    public interface IDataSortRule
    {
        bool CanSort(FieldInfo field);
        int Compare(object leftItem, object rightItem, FieldInfo field);
    }
}
