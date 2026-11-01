using Cysharp.Threading.Tasks;

namespace Template.Features.Ping
{
    public interface IPingProvider
    {
        public UniTask<PingResult> CheckConnection();
    }
}