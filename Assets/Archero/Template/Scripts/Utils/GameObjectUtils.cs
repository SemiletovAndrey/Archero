using UnityEngine;

namespace Template.Scripts.Utils
{
    public static class GameObjectUtils
    {
        public static void TrySetActive(this GameObject gameObject, bool active)
        {
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }
    }
}