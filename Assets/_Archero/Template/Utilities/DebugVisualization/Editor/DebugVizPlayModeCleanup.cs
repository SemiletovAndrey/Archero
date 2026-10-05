#if UNITY_EDITOR
using UnityEditor;

namespace DebugVisualization.Editor
{
    [InitializeOnLoad]
    public static class DebugVizPlayModeCleanup
    {
        static DebugVizPlayModeCleanup()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.EnteredEditMode)
                DebugViz.Shutdown();
        }
    }
}
#endif
