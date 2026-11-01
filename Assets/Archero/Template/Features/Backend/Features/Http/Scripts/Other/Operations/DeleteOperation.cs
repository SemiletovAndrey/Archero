using System.Net.Http;

namespace Template.Features.Backend.Http
{
    public class DeleteOperation : BaseHttpOperation
    {
        public DeleteOperation(HttpClient client) : base(client)
        {
        }

        protected override HttpMethod HttpMethod => HttpMethod.Delete;
    }
}