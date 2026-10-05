#if  UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Rendering.DataRenderers
{
    public interface IDataRenderer
    {
        public VisualElement Render(object data);
        public PropertyField RenderField(object data, FieldInfo field, Action<object> onChanged = null);
        public IReadOnlyList<FieldInfo> GetEditableFields(Type type);
    }
}
#endif
