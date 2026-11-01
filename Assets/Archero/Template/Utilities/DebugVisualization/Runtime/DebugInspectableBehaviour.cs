using UnityEngine;

namespace DebugVisualization
{
    public abstract class DebugInspectableBehaviour : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            DebugViz.Register(this);
        }

        protected virtual void OnDisable()
        {
            DebugViz.Unregister(this);
        }
    }
}
