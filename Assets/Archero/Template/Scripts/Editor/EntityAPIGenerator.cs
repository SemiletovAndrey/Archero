using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Persistance.Features.EntityCore;
using UnityEditor;
using UnityEngine;

namespace Template.PlatformUtils
{
    public class EntityAPIGenerator
    {
        private const string AssemblyName = "Assembly-CSharp";

        private static string OutputPath => Path.Combine(Application.dataPath,
            "Archero/Contexts/Persistance/Features/EntityCore/Scripts/Other/Partial/EntityAPI.cs");

        [MenuItem("Tools/GeneratedEntityApi")]
        private static void Generate()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {typeof(Entity).Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic partial class {typeof(Entity).Name}");
            sb.AppendLine("\t{");

            Assembly assembly = Assembly.Load(AssemblyName);

            IEnumerable<Type> componentTypes = GetComponentTypesFrom(assembly);

            foreach (Type componentType in componentTypes)
            {
                string typeName = componentType.Name;
                string fullTypeName = componentType.FullName;

                string componentName = RemoveSuffixExists(typeName, "Component");
                string modifiedComponentName = componentName + "C";

                sb.AppendLine($"\t\tpublic {fullTypeName} {modifiedComponentName} => GetComponent<{fullTypeName}>();");
                sb.AppendLine();

                if (HasSingleField(componentType, out FieldInfo fieldInfo) && fieldInfo.Name == "Value")
                {
                    sb.AppendLine(
                        $"\t\tpublic {GetValidTypeName(fieldInfo.FieldType)} {componentName} => {modifiedComponentName}.{fieldInfo.Name};");
                    sb.AppendLine();

                    if (HasEmptyConstructor(fieldInfo.FieldType))
                    {
                        string initializer = "{ " + fieldInfo.Name + " = new " + GetValidTypeName(fieldInfo.FieldType) +
                                             "() }";

                        sb.AppendLine($"\t\tpublic {typeof(Entity).FullName} Add{componentName}()");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\treturn AddComponent(new {fullTypeName}() {initializer}); ");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine();
                    }
                }

                string componentParameters = GetParameters(componentType);

                sb.AppendLine($"\t\tpublic {typeof(Entity).FullName} Add{componentName}({componentParameters})");
                sb.AppendLine("\t\t{");
                sb.AppendLine($"\t\t\treturn AddComponent(new {fullTypeName}() {GetInitializer(componentType)});");
                sb.AppendLine("\t\t}");
                sb.AppendLine();
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");


            File.WriteAllText(OutputPath, sb.ToString());

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static bool HasEmptyConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null &&
                   type.IsSubclassOf(typeof(UnityEngine.Object)) == false;
        }

        private static string GetInitializer(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Any() == false)
            {
                return "";
            }

            IEnumerable<string> initializer = fields
                .Select(field => $"{field.Name} = {GetVariableNameFrom(field.Name)}");

            return "{" + string.Join(", ", initializer) + "}";
        }

        private static string GetParameters(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Any() == false)
            {
                return "";
            }

            IEnumerable<string> parameters = fields
                .Select(field => $"{GetValidTypeName(field.FieldType)} {GetVariableNameFrom(field.Name)}");

            return string.Join(", ", parameters);
        }

        private static string GetVariableNameFrom(string fieldName)
        {
            return char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
        }

        private static bool HasSingleField(Type type, out FieldInfo fieldInfo)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (fields.Length != 1)
            {
                fieldInfo = null;
                return false;
            }

            fieldInfo = fields[0];
            return true;
        }

        private static IEnumerable<Type> GetComponentTypesFrom(Assembly assembly)
        {
            IEnumerable<Type> componentTypes = assembly.GetTypes()
                .Where(type => !type.IsInterface
                               && !type.IsAbstract
                               && typeof(IEntityComponent).IsAssignableFrom(type));
            return componentTypes;
        }

        private static string RemoveSuffixExists(string str, string suffix)
        {
            if (str.EndsWith(suffix))
            {
                return str.Substring(0, str.Length - suffix.Length);
            }

            return str;
        }

        public static string GetValidTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();

                string fullTypeName = type.FullName;
                var backIndex = fullTypeName.IndexOf('`');

                if (backIndex >= 0)
                {
                    fullTypeName = fullTypeName.Substring(0, backIndex);
                }

                sb.Append(fullTypeName);
                sb.Append("<");

                Type[] genericArgs = type.GetGenericArguments();

                for (int i = 0; i < genericArgs.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    sb.Append(GetValidTypeName(genericArgs[i]));
                }

                sb.Append(">");
                return sb.ToString();
            }
            else
            {
                return type.FullName;
            }
        }
    }
}