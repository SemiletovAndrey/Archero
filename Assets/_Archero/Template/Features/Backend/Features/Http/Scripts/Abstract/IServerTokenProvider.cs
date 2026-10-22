using Cysharp.Threading.Tasks;

namespace Template.Features.Backend.Http
{
    public interface IServerTokenProvider
    {
        public UniTask<string> GetServerToken();
    }
}