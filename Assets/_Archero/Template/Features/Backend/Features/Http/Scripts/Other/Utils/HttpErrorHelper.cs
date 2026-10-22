using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Template.Scripts.DataHolders;

namespace Template.Features.Backend.Http
{
    public static class HttpErrorHelper
    {
        public static HttpErrorType CreateFromStatusCode(int code)
        {
            return code switch
            {
                >= 500 => HttpErrorType.Server,
                401 => HttpErrorType.Unauthorized,
                403 or >= 400 => HttpErrorType.Client,
                _ => HttpErrorType.Network
            };
        }

        public static HttpErrorType CreateFromException(Exception ex)
        {
            return ex switch
            {
                // ⏱ Timeout
                TaskCanceledException => HttpErrorType.Timeout,
                OperationCanceledException => HttpErrorType.Timeout,

                // 🌐 Network / transport-level
                HttpRequestException httpEx => ClassifyHttpRequestException(httpEx),

                // ❌ Bad URI (your current bug)
                UriFormatException => HttpErrorType.Client,

                // ❌ Serialization issues
                JsonException => HttpErrorType.Client,
                FormatException => HttpErrorType.Client,
                InvalidCastException => HttpErrorType.Client,

                // ❌ Argument / coding issues
                ArgumentException => HttpErrorType.Client,
                InvalidOperationException => HttpErrorType.Client,

                // fallback
                _ => HttpErrorType.Unknown
            };
        }
        
        private static HttpErrorType ClassifyHttpRequestException(HttpRequestException ex)
        {
#if NET5_0_OR_GREATER
    if (ex.StatusCode.HasValue)
    {
        int code = (int)ex.StatusCode.Value;

        if (code >= 500)
            return HttpErrorType.Server;

        if (code >= 400)
            return HttpErrorType.Client;
    }
#endif

            // Inner exception matters A LOT
            if (ex.InnerException is System.Net.Sockets.SocketException)
                return HttpErrorType.Network;

            if (ex.InnerException is System.IO.IOException)
                return HttpErrorType.Network;

            return HttpErrorType.Network;
        }
    }
}