#if  UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Rendering.DataRenderers.Core
{
    public class DefaultDataRenderer : IDataRenderer
    {
        public virtual VisualElement Render(object data)
        {
            var container = new VisualElement();
            foreach (var field in GetEditableFields(data.GetType()))
                container.Add(RenderField(data, field));

            return container;
        }

        public virtual IReadOnlyList<FieldInfo> GetEditableFields(Type type)
        {
            var fields = new List<FieldInfo>();
            var visitedNames = new HashSet<string>();

            for (var current = type; current != null && current != typeof(object); current = current.BaseType)
            {
                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                foreach (var field in current.GetFields(flags))
                {
                    if (field.IsStatic || field.IsNotSerialized)
                        continue;

                    if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                        continue;

                    // Keep the most-derived field when names are hidden in the hierarchy.
                    if (!visitedNames.Add(field.Name))
                        continue;

                    fields.Add(field);
                }
            }

            return fields;
        }

        public virtual PropertyField RenderField(object data, FieldInfo field, Action<object> onChanged = null)
        {
            if (data is UnityEngine.Object unityObject)
            {
                var serializedObject = new SerializedObject(unityObject);
                var property = serializedObject.FindProperty(field.Name);
                if (property != null)
                {
                    var propertyField = new PropertyField(property.Copy());
                    propertyField.label = String.Empty;
                    propertyField.Bind(serializedObject);
                    propertyField.RegisterCallback<SerializedPropertyChangeEvent>(_ =>
                    {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(unityObject);
                        onChanged?.Invoke(field.GetValue(data));
                    });
                    return propertyField;
                }
            }
            return null;
        }
    }
}
#endif