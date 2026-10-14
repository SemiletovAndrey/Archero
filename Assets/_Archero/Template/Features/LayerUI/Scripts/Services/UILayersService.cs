using System.Collections.Generic;
using UnityEngine;

namespace Template.Features.LayerUI
{
    public class UILayersService : IUILayersService
    {
        private readonly Dictionary<UILayerType, (int min, int max)> _ranges = new()
        {
            { UILayerType.Screen, (0, 99) },
            { UILayerType.Popup, (100, 199) },
            { UILayerType.System, (200, 299) }
        };
        
        private readonly Dictionary<UILayerType, List<int>> _used = new();

        private readonly HashSet<UILayerType> _overflowReported = new();

        public int GetNextSortingOrder(UILayerType type)
        {
            (int min, int max) = GetRange(type);
            List<int> used = GetUsed(type);

            // Новый элемент должен быть выше всех открытых: максимум занятых + 1, а в пустом слое - начало диапазона.
            int next = min;
            for (int i = 0; i < used.Count; i++)
            {
                if (used[i] >= next)
                    next = used[i] + 1;
            }

            if (next > max)
            {
                // Больше элементов, чем значений в диапазоне. Порядок совпадет с верхним элементом, а при равенстве
                // выигрывает последний в иерархии - то есть самый новый. Поэтому просто фиксируем максимум.
                if (_overflowReported.Add(type))
                    Debug.LogWarning($"[{nameof(UILayersService)}] Layer {type} is out of sorting orders " +
                                     $"({min}-{max}). Make sure every GetNextSortingOrder has a matching ReleaseSortingOrder.");

                next = max;
            }

            used.Add(next);
            return next;
        }

        public void ReleaseSortingOrder(UILayerType type, int order)
        {
            if (_used.TryGetValue(type, out List<int> used))
                used.Remove(order);
        }

        private (int min, int max) GetRange(UILayerType type)
        {
            if (!_ranges.TryGetValue(type, out (int min, int max) range))
                throw new System.ArgumentOutOfRangeException(nameof(type), type, "Layer has no sorting range.");

            return range;
        }

        private List<int> GetUsed(UILayerType type)
        {
            if (!_used.TryGetValue(type, out List<int> used))
            {
                used = new List<int>();
                _used[type] = used;
            }

            return used;
        }
    }
}