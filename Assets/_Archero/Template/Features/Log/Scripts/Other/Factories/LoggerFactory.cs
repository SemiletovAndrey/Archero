namespace Template.Features.Log
{
    public static class LoggerFactory
    {
        public static IDebugLogger CreateFeatureLogger(string featureName)
        {
            var resolvedStyle = DebugLogStyleResolver.ResolveForFeature(featureName);
            return new FeatureLogger(featureName, resolvedStyle);
        }
    }
}
