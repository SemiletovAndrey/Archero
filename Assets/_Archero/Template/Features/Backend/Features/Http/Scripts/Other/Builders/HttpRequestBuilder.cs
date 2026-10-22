using System;
using System.Collections.Generic;

namespace Template.Features.Backend.Http
{
    public class HttpRequestBuilder<TResult>
    {
        private readonly Func<HttpRequestBuilder<TResult>, TResult> _sendOperation;
            
        public HttpRequestBuilder(Func<HttpRequestBuilder<TResult>, TResult> sendOperation)
        {
            _sendOperation = sendOperation;
        }
        
        public string BaseRoute { get; private set; }
        public string Version { get; private set; }
        public bool UseToken { get; private set; }
        public string Token { get; private set; }
        public bool CheckInternet { get; private set; }
        public bool RepeatUntilOnline { get; private set; }
        public object[] RouteArguments { get; private set; }
        public HttpRequestParameterBuilder Parameters { get; private set; }
        public object Body { get; private set; }
        public IReadOnlyDictionary<string, string> Headers => _headers;
        
        private readonly Dictionary<string, string> _headers = new();

        public TResult Send()
        {
            return _sendOperation(this);
        }
        
        public HttpRequestBuilder<TResult> WithRoute(string value)
        {
            BaseRoute = value;
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithVersion(string value)
        {
            Version = value;
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithTokenUsage(bool value)
        {
            UseToken = value;
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithToken(string value)
        {
            Token = value;
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithInternetCheckStatus(bool checkInternet, bool repeatUntilOnline = true)
        {
            CheckInternet = checkInternet;
            RepeatUntilOnline = repeatUntilOnline;
            
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithRouteArguments(object[] arguments)
        {
            RouteArguments = arguments;
            return this;
        }

        public HttpRequestBuilder<TResult> WithParameters(HttpRequestParameterBuilder parameters)
        {
            Parameters = parameters;
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithBody(object body)
        {
            Body = body;
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Header key cannot be null or empty.", nameof(key));
            }

            _headers[key] = value;
            return this;
        }
        
        public HttpRequestBuilder<TResult> WithHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                return this;
            }

            foreach (var kvp in headers)
            {
                _headers[kvp.Key] = kvp.Value;
            }

            return this;
        }
    }
}