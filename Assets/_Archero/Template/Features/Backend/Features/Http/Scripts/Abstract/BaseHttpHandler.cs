using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Template.Features.Ping;
using Template.Scripts.DataHolders;
using UnityEngine;
using Zenject;

namespace Template.Features.Backend.Http
{
    public abstract class BaseHttpHandler
    {
        private IHttpApiService _apiService;
        private IPingService _pingService;
        
        [Inject]
        private void Construct(IHttpApiService apiService, IPingService pingService)
        {
            _apiService = apiService;
            _pingService = pingService;
        }

        protected virtual string Route => string.Empty;
        protected virtual string Version => BackendConstants.DefaultVersion;
        protected virtual bool UseToken => true;
        protected virtual bool CheckInternet => true;
        protected virtual bool RepeatUntilOnline => true;
        protected virtual IDictionary<string, string> DefaultHeaders => new Dictionary<string, string>
        {
            ["ngrok-skip-browser-warning"] = "true"
        };

        protected virtual string CreateRoute(string version, string route, object[] routeArguments)
        {
            string url = _apiService.CreateUrl(version, route);
            string fullUrl = BackendUtils.CreateFullRouteWithUri(url, routeArguments);
            return fullUrl;
        }
        
        protected virtual async UniTask<string> GetToken(bool useToken)
        {
            return useToken ? await _apiService.GetServerToken() : null;
        }

        protected virtual HttpRequestBuilder<TResult> CreateRequestBuilder<TResult>(Func<HttpRequestBuilder<TResult>, TResult> operation)
        {
            return new HttpRequestBuilder<TResult>(operation)
                .WithRoute(Route)
                .WithVersion(Version)
                .WithTokenUsage(UseToken)
                .WithInternetCheckStatus(CheckInternet, RepeatUntilOnline)
                .WithHeaders(DefaultHeaders);
        }

        protected HttpRequestBuilder<UniTask<Result<T>>> CreateGetRequest<T>(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<T>>>(Get)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<object>>> CreateGetRequest(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<object>>>(Get)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<T>>> CreatePostRequest<T>(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<T>>>(Post)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<object>>> CreatePostRequest(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<object>>>(Post)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<T>>> CreatePutRequest<T>(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<T>>>(Put)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<object>>> CreatePutRequest(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<object>>>(Put)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<T>>> CreatePatchRequest<T>(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<T>>>(Patch)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<object>>> CreatePatchRequest(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<object>>>(Patch)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<T>>> CreateDeleteRequest<T>(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<T>>>(Delete)
                .WithRouteArguments(routeArguments);
        }
        
        protected HttpRequestBuilder<UniTask<Result<object>>> CreateDeleteRequest(params object[] routeArguments)
        {
            return CreateRequestBuilder<UniTask<Result<object>>>(Delete)
                .WithRouteArguments(routeArguments);
        }
        
        private async UniTask<Result<T>> Get<T>(HttpRequestBuilder<UniTask<Result<T>>> builder)
        {
            string route = CreateRoute(builder.Version, builder.BaseRoute, builder.RouteArguments);
            string token = await GetToken(UseToken);

            Debug.Log(
                $"GET {route} \n{JsonConvert.SerializeObject(builder.Parameters)}\n{token}");

            Result<T> requestResult = null;

            await RunRequest(async () =>
            {
                try
                {
                    T result = await _apiService
                        .GetOperation
                        .RequestAsync<T>(route, token, builder.Parameters, headers: builder.Headers);
                    requestResult = Result.Success(result);
                    return requestResult;
                }
                catch (HttpException e)
                {
                    Debug.LogException(e);
                    
                    Error error = HttpError.CreateFromHttpException(e);
                    requestResult = Result.Failure<T>(error);
                    
                    return requestResult;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    
                    Debug.LogError($"[HTTP] Get HttpException: {e.Data} {e.Message}");
                    
                    requestResult = Result.Failure<T>(ErrorFactory.CreateFromException(e));
                    return requestResult;
                }
            }, builder.CheckInternet, builder.RepeatUntilOnline);

            return requestResult;
        }
        
        private async UniTask<Result<T>> Post<T>(HttpRequestBuilder<UniTask<Result<T>>> builder)
        {
            string route = CreateRoute(builder.Version, builder.BaseRoute, builder.RouteArguments);
            string token = await GetToken(UseToken);
            
            Result<T> requestResult = null;
            
            Debug.Log(
                $"POST {route} \n{JsonConvert.SerializeObject(builder.Parameters)}\n{token}");
            
            await RunRequest(async () =>
            {
                try
                {
                    T result = await _apiService
                        .PostOperation
                        .RequestAsync<T>(route, token, builder.Parameters, builder.Body, builder.Headers);

                    requestResult = Result.Success(result);
                    return requestResult;
                }
                catch (HttpException e)
                {
                    Debug.LogException(e);
                    Debug.LogError($"[HTTP] POST HttpException: {e.Type} {e.Message}");
                    
                    Error error = HttpError.CreateFromHttpException(e);
                    requestResult = Result.Failure<T>(error);
                    
                    return requestResult;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError($"[HTTP] POST Exception: {e.GetType().Name} {e.Message}");
                    
                    requestResult = Result.Failure<T>(ErrorFactory.CreateFromException(e));
                    return requestResult;
                }
            }, builder.CheckInternet, builder.RepeatUntilOnline);

            return requestResult;
        }
        
        private async UniTask<Result<T>> Put<T>(HttpRequestBuilder<UniTask<Result<T>>> builder)
        {
            string route = CreateRoute(builder.Version, builder.BaseRoute, builder.RouteArguments);
            string token = await GetToken(UseToken);

            Result<T> requestResult = null;
            
            Debug.Log(
                $"PUT {route} \n{JsonConvert.SerializeObject(builder.Parameters)}\n{token}");
            
            await RunRequest(async () =>
            {
                try
                {
                    T result = await _apiService
                        .PutOperation
                        .RequestAsync<T>(route, token, builder.Parameters, builder.Body, builder.Headers);

                    requestResult = Result.Success(result);
                    return requestResult;
                }
                catch (HttpException e)
                {
                    Debug.LogException(e);
                    
                    Error error = HttpError.CreateFromHttpException(e);
                    requestResult = Result.Failure<T>(error);
                    
                    return requestResult;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    
                    requestResult = Result.Failure<T>(ErrorFactory.CreateFromException(e));
                    return requestResult;
                }
            }, builder.CheckInternet, builder.RepeatUntilOnline);

            return requestResult;
        }

        private async UniTask<Result<T>> Patch<T>(HttpRequestBuilder<UniTask<Result<T>>> builder)
        {
            string route = CreateRoute(builder.Version, builder.BaseRoute, builder.RouteArguments);
            string token = await GetToken(UseToken);
            
            Result<T> requestResult = null;
            
            Debug.Log(
                $"PATCH {route} \n{JsonConvert.SerializeObject(builder.Parameters)}\n{token}");
            
            await RunRequest(async () =>
            {
                try
                {
                    T result = await _apiService
                        .PatchOperation
                        .RequestAsync<T>(route, token, builder.Parameters, builder.Body, builder.Headers);

                    requestResult = Result.Success(result);
                    return requestResult;
                }
                catch (HttpException e)
                {
                    Debug.LogException(e);
                    
                    Error error = HttpError.CreateFromHttpException(e);
                    requestResult = Result.Failure<T>(error);
                    
                    return requestResult;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    
                    requestResult = Result.Failure<T>(ErrorFactory.CreateFromException(e));
                    return requestResult;
                }
            }, builder.CheckInternet, builder.RepeatUntilOnline);

            return requestResult;
        }

        protected async UniTask<Result<T>> Delete<T>(HttpRequestBuilder<UniTask<Result<T>>> builder)
        {
            string route = CreateRoute(builder.Version, builder.BaseRoute, builder.RouteArguments);
            string token = await GetToken(UseToken);

            Result<T> requestResult = null;
            
            Debug.Log(
                $"DELETE {route} \n{JsonConvert.SerializeObject(builder.Parameters)}\n{token}");
            
            await RunRequest(async () =>
            {
                try
                {
                    T result = await _apiService
                        .DeleteOperation
                        .RequestAsync<T>(route, token, builder.Parameters, headers: builder.Headers);

                    requestResult = Result.Success(result);
                    return requestResult;
                }
                catch (HttpException e)
                {
                    Debug.LogException(e);
                    
                    Error error = HttpError.CreateFromHttpException(e);
                    requestResult = Result.Failure<T>(error);
                    
                    return requestResult;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    
                    requestResult = Result.Failure<T>(ErrorFactory.CreateFromException(e));
                    return requestResult;
                }
            }, builder.CheckInternet, builder.RepeatUntilOnline);

            return requestResult;
        }

        private async UniTask<Result> RunRequest(Func<UniTask<Result>> request, bool checkInternet, bool repeatUntilOnline)
        {
            int attempt = 0;
            
            while (true)
            {
                attempt++;
                
                if (checkInternet && !_pingService.HasConnection())
                {
                    await _pingService.WaitForOnline();
                }

                Result result = await request();

                if (result.IsSuccess)
                {
                    return result;
                }

                if (!ShouldRetry(result.Error))
                {
                    Debug.LogWarning($"[HTTP] Request failed without retry: {result.Error.Code} {result.Error.Message}");
                    return result;
                }

                if (!checkInternet && !repeatUntilOnline)
                {
                    Debug.LogWarning($"[HTTP] Request failed and retry disabled: {result.Error.Code} {result.Error.Message}");
                    return result;
                }
            }
        }
        
        private bool ShouldRetry(Error error)
        {
            if (error is not HttpError httpError)
            {
                return false;
            }
            
            return httpError.HttpErrorType switch
            {
                HttpErrorType.Network => true,
                HttpErrorType.Timeout => true,
                HttpErrorType.Server => true,

                _ => false
            };
        }
    }
}
