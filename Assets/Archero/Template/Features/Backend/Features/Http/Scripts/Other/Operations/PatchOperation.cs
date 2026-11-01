using System.Net.Http;

namespace Template.Features.Backend.Http
{
    public class PatchOperation : BaseHttpOperation
    {
        public PatchOperation(HttpClient client) : base(client)
        {
        }

        protected override HttpMethod HttpMethod => HttpMethod.Patch;
    }
}