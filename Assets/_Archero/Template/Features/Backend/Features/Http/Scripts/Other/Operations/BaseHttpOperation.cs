using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

namespace Template.Features.Backend.Http
{
    public abstract class BaseHttpOperation
    {
        private readonly HttpClient _client;

        protected BaseHttpOperation(HttpClient client)
        {
            _client = client;
        }

        protected abstract HttpMethod HttpMethod { get; }

        public UniTask<T> RequestAsync<T>(string endpoint, string token = null,
            Dictionary<string, object> parameters = null, object data = null,
            IReadOnlyDictionary<string, string> headers = null)
        {
            HttpRequestMessage request;
            try
            {
                request = FinalizeRequest(endpoint, token, parameters, data, headers);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[HTTP] FinalizeRequest failed: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                HttpErrorType type = HttpErrorHelper.CreateFromException(ex);
                throw new HttpException(type, ex.Message);
            }

            return RequestAsync<T>(request);
        }
        
        private HttpRequestMessage FinalizeRequest(string endpoint, string token, 
            Dictionary<string, object> parameters, object data,
            IReadOnlyDictionary<string, string> headers)
        {
            if (parameters is { Count: > 0 })
            {
                string query = string.Join("&", parameters.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value?.ToString() ?? string.Empty)}"
                ));

                endpoint += "?" + query;
            }

            HttpRequestMessage request = new(HttpMethod, endpoint);
            request.Headers.Add("Accept", "application/json");

            if (token != null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
            }

            if (headers is { Count: > 0 })
            {
                foreach (var kvp in headers)
                {
                    if (string.IsNullOrEmpty(kvp.Key))
                    {
                        continue;
                    }
                    
                    request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }
            }

            if (data != null)
            {
                request.Content = CreateJsonContent(data);
            }

            return request;
        }
        
        private async UniTask<T> RequestAsync<T>(HttpRequestMessage request)
        {
            try
            {
                Debug.Log($"[HTTP] Request: {request.Method} {request.RequestUri}");
                
                if (request.Content != null)
                {
                    string requestBody = await request.Content.ReadAsStringAsync();
                    Debug.Log($"[HTTP] Request body: {requestBody}");
                }

                Debug.Log("[HTTP] Sending request...");
                HttpResponseMessage response = await _client.SendAsync(request);
                Debug.Log($"[HTTP] Response status: {(int)response.StatusCode} {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    int code = (int)response.StatusCode;

                    HttpErrorType type = HttpErrorHelper.CreateFromStatusCode(code);
                    string errorBody = await response.Content.ReadAsStringAsync();
                    Debug.LogError($"[HTTP] Error response body: {errorBody}");
                    
                    throw new HttpException(type, errorBody);
                }

                string json = await response.Content.ReadAsStringAsync();

                Debug.Log($"[HTTP] Response body: {json}");
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (HttpException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[HTTP] Request exception: {ex.GetType().Name}: {ex.Message}");
                HttpErrorType type = HttpErrorHelper.CreateFromException(ex);
                throw new HttpException(type, ex.Message);
            }
        }
        
        private StringContent CreateJsonContent(object data)
        {
            string json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
