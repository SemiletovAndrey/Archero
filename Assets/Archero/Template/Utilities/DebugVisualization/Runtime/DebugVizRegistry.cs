using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DebugVisualization
{
    internal static class DebugVizRegistry
    {
        private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly List<RegisteredTarget> Targets = new();
        private static readonly HashSet<Type> InspectableTypes = new();
        private static bool _typeCacheBuilt;

        public static void Register(UnityEngine.Object target)
        {
            if (target == null || Contains(target))
                return;

            var inspectedMembers = BuildInspectedMembers(target.GetType());
            var drawable = target as IDebugDrawable;

            if (inspectedMembers.Count == 0 && drawable == null)
                return;

            Targets.Add(new RegisteredTarget(target, inspectedMembers, drawable));
        }

        public static void AutoRegisterSceneTargets()
        {
            EnsureTypeCache();

            if (InspectableTypes.Count == 0)
                return;

            var behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            for (var i = 0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];

                if (behaviour == null)
                    continue;

                var type = behaviour.GetType();
                if (InspectableTypes.Contains(type))
                    Register(behaviour);
            }
        }

        public static void Unregister(UnityEngine.Object target)
        {
            for (var i = Targets.Count - 1; i >= 0; i--)
            {
                if (Targets[i].Target == target)
                    Targets.RemoveAt(i);
            }
        }

        public static void Clear()
        {
            Targets.Clear();
        }

        public static void Refresh()
        {
            for (var i = Targets.Count - 1; i >= 0; i--)
            {
                var registered = Targets[i];

                if (registered.Target == null)
                {
                    Targets.RemoveAt(i);
                    continue;
                }

                registered.Drawable?.DrawDebug(new DebugVizContext(registered.Target));

                for (var memberIndex = 0; memberIndex < registered.Members.Count; memberIndex++)
                    registered.Members[memberIndex].Draw(registered.Target);
            }
        }

        private static bool Contains(UnityEngine.Object target)
        {
            for (var i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].Target == target)
                    return true;
            }

            return false;
        }

        private static List<InspectedMember> BuildInspectedMembers(Type type)
        {
            var result = new List<InspectedMember>();

            foreach (var field in type.GetFields(MemberFlags))
                TryAdd(result, field, field.FieldType, target => field.GetValue(target));

            foreach (var property in type.GetProperties(MemberFlags))
            {
                if (property.GetIndexParameters().Length > 0 || property.GetMethod == null)
                    continue;

                TryAdd(result, property, property.PropertyType, target => property.GetValue(target));
            }

            return result;
        }

        private static void EnsureTypeCache()
        {
            if (_typeCacheBuilt)
                return;

            _typeCacheBuilt = true;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in GetLoadableTypes(assembly))
                {
                    if (type == null || type.IsAbstract || !typeof(MonoBehaviour).IsAssignableFrom(type))
                        continue;

                    if (typeof(IDebugDrawable).IsAssignableFrom(type) || BuildInspectedMembers(type).Count > 0)
                        InspectableTypes.Add(type);
                }
            }
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(type => type != null);
            }
        }

        private static void TryAdd(List<InspectedMember> members, MemberInfo member, Type valueType, Func<object, object> getter)
        {
            var valueAttribute = member.GetCustomAttribute<DebugValueAttribute>();
            if (valueAttribute != null)
            {
                members.Add(InspectedMember.Value(member.Name, valueAttribute, getter));
                return;
            }

            var radiusAttribute = member.GetCustomAttribute<DebugRadiusAttribute>();
            if (radiusAttribute != null && IsNumeric(valueType))
                members.Add(InspectedMember.Radius(member.Name, radiusAttribute, getter));
        }

        private static bool IsNumeric(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(byte)
                   || type == typeof(sbyte)
                   || type == typeof(short)
                   || type == typeof(ushort)
                   || type == typeof(int)
                   || type == typeof(uint)
                   || type == typeof(long)
                   || type == typeof(ulong)
                   || type == typeof(float)
                   || type == typeof(double)
                   || type == typeof(decimal);
        }

        private readonly struct RegisteredTarget
        {
            public readonly UnityEngine.Object Target;
            public readonly List<InspectedMember> Members;
            public readonly IDebugDrawable Drawable;

            public RegisteredTarget(UnityEngine.Object target, List<InspectedMember> members, IDebugDrawable drawable)
            {
                Target = target;
                Members = members;
                Drawable = drawable;
            }
        }

        private readonly struct InspectedMember
        {
            private readonly string _label;
            private readonly string _channel;
            private readonly bool _isRadius;
            private readonly bool _filledRadius;
            private readonly DebugVizValueDisplay _display;
            private readonly float _worldYOffset;
            private readonly Func<object, object> _getter;

            private InspectedMember(
                string fallbackLabel,
                string label,
                string channel,
                bool isRadius,
                bool filledRadius,
                DebugVizValueDisplay display,
                float worldYOffset,
                Func<object, object> getter)
            {
                _label = string.IsNullOrWhiteSpace(label) ? fallbackLabel : label;
                _channel = string.IsNullOrWhiteSpace(channel) ? DebugViz.DefaultChannel : channel;
                _isRadius = isRadius;
                _filledRadius = filledRadius;
                _display = display;
                _worldYOffset = worldYOffset;
                _getter = getter;
            }

            public static InspectedMember Value(string fallbackLabel, DebugValueAttribute attribute, Func<object, object> getter)
            {
                return new InspectedMember(fallbackLabel, attribute.Label, attribute.Channel, false, false, attribute.Display, attribute.WorldYOffset, getter);
            }

            public static InspectedMember Radius(string fallbackLabel, DebugRadiusAttribute attribute, Func<object, object> getter)
            {
                return new InspectedMember(fallbackLabel, attribute.Label, attribute.Channel, true, attribute.Filled, attribute.ValueDisplay, attribute.WorldYOffset, getter);
            }

            public void Draw(UnityEngine.Object target)
            {
                object value;

                try
                {
                    value = _getter(target);
                }
                catch (Exception exception)
                {
                    DrawValue(target, exception.GetType().Name);
                    return;
                }

                DrawValue(target, value);

                if (!_isRadius || !TryGetPosition(target, out var position) || !TryConvertFloat(value, out var radius))
                    return;

                if (_filledRadius)
                    DebugViz.DrawZoneDisc(position, radius, _channel, duration: 0f);
                else
                    DebugViz.DrawRadius(position, radius, _channel, duration: 0f);
            }

            private void DrawValue(UnityEngine.Object target, object value)
            {
                if ((_display & DebugVizValueDisplay.Hud) != 0)
                    DebugVizRuntime.AddPersistentValue(target, _channel, _label, value);

                if ((_display & DebugVizValueDisplay.World) == 0 || !TryGetPosition(target, out var position))
                    return;

                var labelPosition = position + Vector3.up * Mathf.Max(0f, _worldYOffset);
                DebugVizRuntime.AddPersistentWorldLabel(target, _channel, _label, value, labelPosition);
            }

            private static bool TryGetPosition(UnityEngine.Object target, out Vector3 position)
            {
                if (target is Component component)
                {
                    position = component.transform.position;
                    return true;
                }

                if (target is GameObject gameObject)
                {
                    position = gameObject.transform.position;
                    return true;
                }

                position = default;
                return false;
            }

            private static bool TryConvertFloat(object value, out float result)
            {
                try
                {
                    result = Convert.ToSingle(value);
                    return true;
                }
                catch
                {
                    result = 0f;
                    return false;
                }
            }
        }
    }
}
