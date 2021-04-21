using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TravelSystemIntegration.Helpers
{
    public class HttpClientHelper<T>
    {
        private HttpClient _client;

        public HttpClientHelper(string authHeader)
        {
            this._client = new HttpClient();
            this.SetAuthHeader(authHeader);
        }

        private void SetAuthHeader(string authHeader)
        {
            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                _client.DefaultRequestHeaders.Add("Authorization", $"Basic {authHeader}");
            }
        }

        public async Task<T> GetSingleItemRequest(string apiUrl, CancellationToken cancellationToken)
        {
            var result = default(T);
            var response = await _client.GetAsync(apiUrl, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync().ContinueWith(x =>
                {
                    if (typeof(T).Namespace != "System")
                    {
                        result = JsonConvert.DeserializeObject<T>(x?.Result);
                    }
                    else result = (T)Convert.ChangeType(x?.Result, typeof(T));
                }, cancellationToken);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                response.Content?.Dispose();
                throw new HttpRequestException($"{response.StatusCode}:{content}");
            }
            return result;
        }


        public async Task<T> PostRequest(string apiUrl, T postObject, CancellationToken cancellationToken)
        {
            T result = default(T);
            var body = JsonConvert.SerializeObject(postObject);
            var response = await _client.PostAsync(apiUrl, body != null ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    result = JsonConvert.DeserializeObject<T>(x.Result);
                }, cancellationToken);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                response.Content?.Dispose();
                throw new HttpRequestException($"{response.StatusCode}:{content}");
            }
            return result;
        }

        public async Task PutRequest(string apiUrl, T putObject, CancellationToken cancellationToken)
        {
            var body = JsonConvert.SerializeObject(putObject);
            var response = await _client.PutAsync(apiUrl, body != null ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                response.Content?.Dispose();
                throw new HttpRequestException($"{response.StatusCode}:{content}");
            }
        }

        public async Task PatchRequest(string apiUrl, T patchObject, CancellationToken cancellationToken)
        {
            var body = JsonConvert.SerializeObject(patchObject);
            var response = await _client.PatchAsync(apiUrl, body != null ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                response.Content?.Dispose();
                throw new HttpRequestException($"{response.StatusCode}:{content}");
            }
        }

    }
}
