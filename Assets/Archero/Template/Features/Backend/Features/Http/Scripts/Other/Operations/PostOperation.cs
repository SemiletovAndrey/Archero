using System.Net.Http;

namespace Template.Features.Backend.Http
{
    public class PostOperation : BaseHttpOperation
    {
        public PostOperation(HttpClient client) : base(client)
        {
        }
        
        protected override HttpMethod HttpMethod => HttpMethod.Post;
        
        
    }
}