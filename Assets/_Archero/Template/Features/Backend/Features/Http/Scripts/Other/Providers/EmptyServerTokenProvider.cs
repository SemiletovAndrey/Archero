using Cysharp.Threading.Tasks;

namespace Template.Features.Backend.Http
{
    public class EmptyServerTokenProvider : IServerTokenProvider
    {
        public UniTask<string> GetServerToken()
        {
            return  UniTask.FromResult("");
        }
    }
}