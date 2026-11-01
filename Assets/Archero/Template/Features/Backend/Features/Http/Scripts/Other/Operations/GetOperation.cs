using System.Net.Http;

namespace Template.Features.Backend.Http
{
    public class GetOperation : BaseHttpOperation
    {
        public GetOperation(HttpClient client) : base(client)
        {
        }

        protected override HttpMethod HttpMethod => HttpMethod.Get;
    }
}