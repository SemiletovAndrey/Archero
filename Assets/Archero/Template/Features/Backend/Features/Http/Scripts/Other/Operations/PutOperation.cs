using System.Net.Http;

namespace Template.Features.Backend.Http
{
    public class PutOperation : BaseHttpOperation
    {
        public PutOperation(HttpClient client) : base(client)
        {
        }

        protected override HttpMethod HttpMethod => HttpMethod.Put;
    }
}