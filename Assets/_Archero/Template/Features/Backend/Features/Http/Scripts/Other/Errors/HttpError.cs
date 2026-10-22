using Template.Scripts.DataHolders;

namespace Template.Features.Backend.Http
{
    public class HttpError : Error
    {
        public HttpError(HttpErrorType httpErrorType, string message) : base($"Http.{httpErrorType}", message)
        {
            HttpErrorType = httpErrorType;
        }
        
        public HttpErrorType HttpErrorType { get; }

        public static HttpError CreateFromHttpException(HttpException exception)
        {
            return new HttpError(exception.Type, exception.Message);
        }
    }
}