using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Template.Features.Ping
{
    public class WebRequestPingProvider : IPingProvider
    {
        public async UniTask<PingResult> CheckConnection()
        {
            using var request = UnityWebRequest.Head(PingConstants.GoogleLifeCheckLink);

            float start = Time.realtimeSinceStartup;

            await request.SendWebRequest();

            float duration = Time.realtimeSinceStartup - start;

            if (request.result != UnityWebRequest.Result.Success)
                return PingResult.Failed;

            return duration > PingConstants.SlowConnectionThreshold
                ? PingResult.Slow
                : PingResult.Success;
        }
    }
}