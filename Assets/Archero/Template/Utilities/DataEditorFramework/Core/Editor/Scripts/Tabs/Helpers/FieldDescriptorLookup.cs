#if  UNITY_EDITOR
using System.Collections.Generic;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal static class FieldDescriptorLookup
    {
        public static FieldDescriptor? GetByKey(string key, IReadOnlyList<FieldDescriptor> descriptors)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            for (var i = 0; i < descriptors.Count; i++)
            {
                if (descriptors[i].Key == key)
                    return descriptors[i];
            }

            return null;
        }
    }
}
#endif