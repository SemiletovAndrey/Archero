using System.Linq;

public static class BackendUtils
{
    public static string ConcatRoute(string serverURL, string version, string controller, params object[] extraRoute)
    {
        string[] routes = { serverURL, version, controller };
        routes = routes.Concat(extraRoute).Select(x => x.ToString()).ToArray();

        return JoinRoutes(routes);
    }
    
    public static string ConcatRoute(params object[] routes)
    {
        string[] stringRoutes = routes.Select(x => x.ToString()).ToArray();
        return JoinRoutes(stringRoutes);
    }
    
    public static string CreateFullRouteWithUri(string uri, params object[] extraRoute)
    {
        string[] routes = { uri };
        routes = routes.Concat(extraRoute).Select(x => x.ToString()).ToArray();

        return JoinRoutes(routes);
    }

    private static string JoinRoutes(params string[] routeParts)
    {
        return string.Join('/', routeParts);
    }
}
