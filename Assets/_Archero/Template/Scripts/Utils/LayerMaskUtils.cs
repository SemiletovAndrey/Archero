using UnityEngine;

namespace Template.Scripts.Utils
{
    public static class LayerMaskUtils
    {
        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }
    }
}