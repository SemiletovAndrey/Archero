using Cysharp.Threading.Tasks;

namespace Template.Features.Backend.Http
{
    public interface IHttpApiService
    {
        PostOperation PostOperation { get; }
        GetOperation GetOperation { get; }
        PutOperation PutOperation { get; }
        PatchOperation PatchOperation { get; }
        DeleteOperation DeleteOperation { get; }

        public string CreateUrl(string version, string route);
        public UniTask<string> GetServerToken();

        public void AddUrlProvider(IHttpUrlProvider provider);
        public void RemoveUrlProvider(IHttpUrlProvider provider);
        
        public void AddTokenProvider(IServerTokenProvider provider);
        public void RemoveTokenProvider(IServerTokenProvider provider);
    }
}