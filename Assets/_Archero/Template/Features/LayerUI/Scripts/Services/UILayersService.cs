using System.Collections.Generic;

namespace Template.Features.LayerUI
{
    public class UILayersService : IUILayersService
    {
        private readonly Dictionary<UILayerType, (int min, int max)> _ranges = new()
        {
            { UILayerType.Screen, (0, 99) },
            { UILayerType.Popup,  (100, 199) },
            { UILayerType.System, (200, 299) }
        };

        private readonly Dictionary<UILayerType, int> _currentMax = new();

        public int GetNextSortingOrder(UILayerType type)
        {
            if (!_currentMax.ContainsKey(type))
                _currentMax[type] = _ranges[type].min;

            int next = _currentMax[type];
            if (next > _ranges[type].max)
            {
                next = _ranges[type].min;
                _currentMax[type] = next;
            }
            _currentMax[type] = next + 1;
            return next;
        }
    }
}