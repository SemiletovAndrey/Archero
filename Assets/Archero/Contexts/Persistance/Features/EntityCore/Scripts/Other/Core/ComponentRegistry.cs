using System;
using System.Collections.Generic;

namespace Persistance.Features.EntityCore
{
    internal static class ComponentRegistry
    {
        private static readonly Dictionary<Type, int> Ids = new();

        public static int Count => Ids.Count;

        public static int GetId(Type type)
        {
            if (!Ids.TryGetValue(type, out int id))
            {
                id = Ids.Count;
                Ids.Add(type, id);
            }

            return id;
        }
    }

    /// <summary>Кэш id для generic-пути: после первого обращения это просто чтение статического поля.</summary>
    internal static class ComponentId<T>
    {
        public static readonly int Value = ComponentRegistry.GetId(typeof(T));
    }
}
