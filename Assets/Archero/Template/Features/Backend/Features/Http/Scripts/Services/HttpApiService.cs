using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.Backend.Http
{
    public class HttpApiService : IHttpApiService
    {
        private readonly HttpClient _httpClient = new();
        
        private readonly List<IServerTokenProvider> _tokenProviders = new();
        private readonly List<IHttpUrlProvider> _urlProviders = new();

        public HttpApiService()
        {
        }

        public PostOperation PostOperation { get; private set; }
        public GetOperation GetOperation { get; private set; }
        public PutOperation PutOperation { get; private set; }
        public PatchOperation PatchOperation { get; private set; }
        public DeleteOperation DeleteOperation { get; private set; }

        public void Init()
        {
            CreateOperations();
        }

        public string CreateUrl(string version, string route)
        {
            return _urlProviders.Last().CreateUrl(version, route);
        }

        public async UniTask<string> GetServerToken()
        {
            return await _tokenProviders.Last().GetServerToken();
        }

        public void AddUrlProvider(IHttpUrlProvider provider)
        {
            _urlProviders.Add(provider);
        }

        public void RemoveUrlProvider(IHttpUrlProvider provider)
        {
            _urlProviders.Remove(provider);
        }

        public void AddTokenProvider(IServerTokenProvider provider)
        {
            _tokenProviders.Add(provider);
        }

        public void RemoveTokenProvider(IServerTokenProvider provider)
        {
            _tokenProviders.Remove(provider);
        }

        private void CreateOperations()
        {
            PostOperation = new PostOperation(_httpClient);
            GetOperation = new GetOperation(_httpClient);
            PutOperation = new PutOperation(_httpClient);
            PatchOperation = new PatchOperation(_httpClient);
            DeleteOperation = new DeleteOperation(_httpClient);
        }
    }
}