using System;
using Cysharp.Threading.Tasks;
using Template.Features.Backend.Http;
using Template.Features.Ping;
using Template.Scripts.DataHolders;
using UnityEngine;

namespace Template.Features.Backend.Ping
{
    public class HttpPingProvider : BaseHttpHandler, IPingProvider
    {
        protected override string Route => "time";
        protected override bool UseToken => false;
        protected override bool CheckInternet => false;

        public async UniTask<PingResult> CheckConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return PingResult.Failed;
            }

            Result<DateTime> result = await CreateGetRequest<DateTime>().Send();
            return result.IsSuccess ? PingResult.Success : PingResult.Failed;
        }
    }
}