#if  UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataEditorFramework.Core.Rendering.Attribute;

namespace DataEditorFramework.Core.Editor.Scripts.Rendering.DataRenderers.Core
{
    public static class DataRendererRegistry
    {
        private static Dictionary<Type, IDataRenderer> _cache;
        private static IDataRenderer _default;

        static DataRendererRegistry()
        {
            BuildCache();
        }

        private static void BuildCache()
        {
            _cache = new Dictionary<Type, IDataRenderer>();

            _default = new DefaultDataRenderer();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    typeof(IDataRenderer).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<DataRendererAttribute>();
                if (attr == null)
                    continue;

                var instance = (IDataRenderer)Activator.CreateInstance(type);

                _cache[attr.TargetType] = instance;
            }
        }

        public static IDataRenderer GetRenderer(Type type)
        {
            return _cache.GetValueOrDefault(type, _default);
        }
    }
}
#endif