using System.Collections.Generic;
using UnityEngine;

namespace DebugVisualization
{
    internal static class DebugVizRuntime
    {
        private const string SettingsResourcePath = "DebugVizSettings";

        private static DebugVizRunner _runner;
        private static DebugVizSettings _settings;
        private static readonly DebugVizSettings _fallbackSettings = ScriptableObject.CreateInstance<DebugVizSettings>();

        public static DebugVizSettings Settings
        {
            get
            {
                EnsureInitialized();
                return _settings != null ? _settings : _fallbackSettings;
            }
        }

        internal static DebugVizRunner Runner
        {
            get
            {
                EnsureInitialized();
                return _runner;
            }
        }

        public static bool HudVisible
        {
            get
            {
                EnsureInitialized();
                return _runner != null && _runner.HudVisible;
            }
            set
            {
                EnsureInitialized();
                if (_runner != null)
                    _runner.HudVisible = value;
            }
        }

        public static void Add(DebugVizDrawCommand command)
        {
            EnsureInitialized();
            _runner.Add(command);
        }

        public static void AddPersistentValue(Object owner, string channel, string label, object value)
        {
            EnsureInitialized();
            _runner.AddPersistentValue(owner, channel, label, value);
        }

        public static void AddPersistentWorldLabel(Object owner, string channel, string label, object value, Vector3 position)
        {
            EnsureInitialized();
            _runner.AddPersistentWorldLabel(owner, channel, label, value, position);
        }

        public static void SetChannelEnabled(string channel, bool enabled)
        {
            EnsureInitialized();
            _runner.SetChannelEnabled(channel, enabled);
        }

        public static bool IsChannelEnabled(string channel)
        {
            EnsureInitialized();
            return _runner.IsChannelEnabled(channel);
        }

        public static IReadOnlyCollection<string> Channels
        {
            get
            {
                EnsureInitialized();
                return _runner.Channels;
            }
        }

        public static void ClearAll()
        {
            DebugVizRegistry.Clear();

            if (_runner != null)
                _runner.ClearAll();
        }

        public static void ClearDraws()
        {
            EnsureInitialized();
            _runner.ClearDraws();
        }

        public static void Shutdown()
        {
            DebugVizRegistry.Clear();

            var runners = Resources.FindObjectsOfTypeAll<DebugVizRunner>();
            for (var i = 0; i < runners.Length; i++)
                DestroyRunner(runners[i]);

            _runner = null;
            _settings = null;
        }

        private static void DestroyRunner(DebugVizRunner runner)
        {
            if (runner == null)
                return;

            var host = runner.gameObject;
            runner.ClearAll();

            if (host == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(host);
            else
                Object.DestroyImmediate(host);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD || DEBUG_VISUALS
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            EnsureInitialized();
        }
#endif

        private static void EnsureInitialized()
        {
            if (_runner != null)
                return;

            _settings = Resources.Load<DebugVizSettings>(SettingsResourcePath);

            var host = new GameObject("[Debug Visualization]");
            Object.DontDestroyOnLoad(host);
            host.hideFlags = HideFlags.HideAndDontSave;

            _runner = host.AddComponent<DebugVizRunner>();
            _runner.Initialize(_settings != null ? _settings : _fallbackSettings);
        }
    }
}
