using System;

namespace Template.Features.Backend.Http
{
    public class HttpException : Exception
    {
        public HttpErrorType Type { get; }

        public HttpException(HttpErrorType type, string message) : base(message)
        {
            Type = type;
        }
    }
}