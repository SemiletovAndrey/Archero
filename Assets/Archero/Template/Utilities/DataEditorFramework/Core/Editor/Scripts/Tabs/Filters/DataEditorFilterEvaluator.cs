#if  UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal sealed class DataEditorFilterEvaluation
    {
        public readonly Dictionary<object, HashSet<string>> ItemFailedFields = new();
        public readonly HashSet<object> MismatchedItems = new();
    }

    internal static class DataEditorFilterEvaluator
    {
        public static void EvaluateVisibleItems(
            IList sourceItems,
            List<object> visibleItems,
            IReadOnlyList<FieldDescriptor> descriptors,
            IReadOnlyList<FilterCondition> filterConditions,
            string searchText,
            FilterEffectMode filterMode,
            bool wholeConfig,
            DataEditorFilterEvaluation result)
        {
            visibleItems.Clear();
            result.ItemFailedFields.Clear();
            result.MismatchedItems.Clear();

            var candidates = BuildSearchCandidates(sourceItems, searchText);
            EvaluateFilterFailures(candidates, descriptors, filterConditions, result);

            if (filterMode == FilterEffectMode.Hide && wholeConfig)
            {
                foreach (var candidate in candidates)
                {
                    if (!result.MismatchedItems.Contains(candidate))
                        visibleItems.Add(candidate);
                }
            }
            else
            {
                visibleItems.AddRange(candidates);
            }
        }

        public static IReadOnlyList<FilterOperator> GetAllowedOperators(Type fieldType)
        {
            if (fieldType == null)
                return EqualityOperators;

            var underlying = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
            if (!typeof(IComparable).IsAssignableFrom(underlying))
                return EqualityOperators;

            return ComparableOperators;
        }

        public static bool IsOperatorAllowedForField(Type fieldType, FilterOperator op)
        {
            return GetAllowedOperators(fieldType).Contains(op);
        }

        public static string GetOperatorLabel(FilterOperator op)
        {
            return op switch
            {
                FilterOperator.Less => "<",
                FilterOperator.Greater => ">",
                FilterOperator.GreaterOrEqual => ">=",
                FilterOperator.LessOrEqual => "<=",
                FilterOperator.Equal => "==",
                FilterOperator.NotEqual => "!=",
                _ => "=="
            };
        }

        public static bool IsConditionActive(FilterCondition condition)
        {
            return !string.IsNullOrWhiteSpace(condition.FieldKey)
                   && condition.ValueCarrier != null
                   && !string.IsNullOrWhiteSpace(condition.ActiveValuePropertyPath);
        }

        private static readonly FilterOperator[] EqualityOperators =
        {
            FilterOperator.Equal,
            FilterOperator.NotEqual
        };

        private static readonly FilterOperator[] ComparableOperators =
        {
            FilterOperator.Less,
            FilterOperator.Greater,
            FilterOperator.GreaterOrEqual,
            FilterOperator.LessOrEqual,
            FilterOperator.Equal,
            FilterOperator.NotEqual
        };

        private static List<object> BuildSearchCandidates(IList sourceItems, string searchText)
        {
            var hasSearch = !string.IsNullOrWhiteSpace(searchText);
            var normalizedSearch = hasSearch ? searchText.Trim() : string.Empty;
            var candidates = new List<object>();

            foreach (var item in sourceItems)
            {
                if (item == null)
                    continue;

                if (hasSearch)
                {
                    if (item is not UnityEngine.Object unityObject)
                        continue;

                    var configName = unityObject.name ?? string.Empty;
                    if (configName.IndexOf(normalizedSearch, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                }

                candidates.Add(item);
            }

            return candidates;
        }

        private static void EvaluateFilterFailures(
            IEnumerable<object> candidates,
            IReadOnlyList<FieldDescriptor> descriptors,
            IReadOnlyList<FilterCondition> filterConditions,
            DataEditorFilterEvaluation result)
        {
            foreach (var item in candidates)
            {
                var failedFields = new HashSet<string>();
                foreach (var condition in filterConditions)
                {
                    if (!IsConditionActive(condition))
                        continue;

                    var descriptor = FieldDescriptorLookup.GetByKey(condition.FieldKey, descriptors);
                    if (!descriptor.HasValue)
                        continue;

                    var isMatch = TryEvaluateCondition(item, descriptor.Value, condition);
                    if (!isMatch)
                        failedFields.Add(descriptor.Value.Key);
                }

                if (failedFields.Count == 0)
                    continue;

                result.ItemFailedFields[item] = failedFields;
                result.MismatchedItems.Add(item);
            }
        }

        private static bool TryEvaluateCondition(object item, FieldDescriptor descriptor, FilterCondition condition)
        {
            if (item is not UnityEngine.Object unityObject || condition.ValueCarrier == null || string.IsNullOrEmpty(condition.ActiveValuePropertyPath))
                return false;

            var leftSo = new SerializedObject(unityObject);
            var leftProperty = leftSo.FindProperty(descriptor.Field.Name);
            if (leftProperty == null)
                return false;

            var rightSo = new SerializedObject(condition.ValueCarrier);
            var rightProperty = rightSo.FindProperty(condition.ActiveValuePropertyPath);
            if (rightProperty == null)
                return false;

            if (!TryGetComparableValue(leftProperty, out var leftValue, out var leftType) ||
                !TryGetComparableValue(rightProperty, out var rightValue, out var rightType))
                return false;

            var compareType = leftType ?? rightType;
            if (compareType == null)
                return false;

            return CompareValues(leftValue, rightValue, compareType, condition.Operator);
        }

        private static bool CompareValues(object left, object right, Type valueType, FilterOperator op)
        {
            if (op == FilterOperator.Equal)
                return Equals(left, right);
            if (op == FilterOperator.NotEqual)
                return !Equals(left, right);

            if (left == null || right == null)
                return false;

            var underlying = Nullable.GetUnderlyingType(valueType) ?? valueType;
            if (left.GetType() != underlying && right.GetType() == underlying)
                left = Convert.ChangeType(left, underlying);
            if (right.GetType() != underlying && left.GetType() == underlying)
                right = Convert.ChangeType(right, underlying);

            if (left is not IComparable leftComparable || right is not IComparable rightComparable)
                return false;

            int result;
            try
            {
                result = leftComparable.CompareTo(rightComparable);
            }
            catch
            {
                return false;
            }

            return op switch
            {
                FilterOperator.Greater => result > 0,
                FilterOperator.Less => result < 0,
                FilterOperator.GreaterOrEqual => result >= 0,
                FilterOperator.LessOrEqual => result <= 0,
                _ => false
            };
        }

        private static bool TryGetComparableValue(SerializedProperty property, out object value, out Type valueType)
        {
            value = null;
            valueType = null;

            if (property == null)
                return false;

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                value = property.objectReferenceValue;
                valueType = typeof(UnityEngine.Object);
                return true;
            }

            try
            {
                value = property.boxedValue;
                valueType = value?.GetType() ?? GetFieldTypeFromProperty(property);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static Type GetFieldTypeFromProperty(SerializedProperty property)
        {
            var targetType = property.serializedObject.targetObject?.GetType();
            if (targetType == null)
                return null;

            var field = targetType.GetField(property.name,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            return field?.FieldType;
        }
    }
}
#endif