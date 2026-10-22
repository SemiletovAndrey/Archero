using System.Collections.Generic;

namespace Template.Features.Backend.Http
{
    public class HttpRequestParameterBuilder
    {
        private readonly Dictionary<string, object> _parameters;

        public HttpRequestParameterBuilder()
        {
            _parameters = new Dictionary<string, object>();
        }

        public HttpRequestParameterBuilder(Dictionary<string, object> parameters)
        {
            _parameters = parameters;
        }
        
        public HttpRequestParameterBuilder AddParameter(string name, object value)
        {
            _parameters[name] = value;
            return this;
        }

        public static implicit operator Dictionary<string, object>(HttpRequestParameterBuilder builder)
        {
            return builder?._parameters;
        }
        
        public static implicit operator HttpRequestParameterBuilder(Dictionary<string, object> parameters)
        {
            return parameters == null ? null : new HttpRequestParameterBuilder(parameters);
        }
    }
}