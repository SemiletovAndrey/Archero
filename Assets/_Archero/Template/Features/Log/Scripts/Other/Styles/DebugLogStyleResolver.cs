using UnityEngine;

namespace Template.Features.Log
{
    public static class DebugLogStyleResolver
    {
        private const string SettingsResourcePath = "DebugLogSettings";
        private static DebugLogSettings _settings;

        public static DebugLogStyleData ResolveForFeature(string featureName)
        {
            _settings ??= Resources.Load<DebugLogSettings>(SettingsResourcePath);

            if (_settings == null)
                return DebugLogStyleData.Default;

            var resolved = _settings.GetStyle(featureName);
            return (resolved ?? _settings.DefaultStyle ?? DebugLogStyleData.Default).Clone();
        }
    }
}
