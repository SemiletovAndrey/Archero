using Cysharp.Threading.Tasks;

namespace Template.Features.Ping
{
    public class EmptyPingProvider : IPingProvider
    {
        public UniTask<PingResult> CheckConnection()
        {
            return UniTask.FromResult(PingResult.Success);
        }
    }
}