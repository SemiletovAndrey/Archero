namespace Template.Features.Backend.Http
{
    public interface IHttpUrlProvider
    {
        public string CreateUrl(string version, string route);
    }
}