using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApp.Proxy
{
    public class ApiClient
    {
        private static readonly JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly HttpClient _httpClient;
        private readonly IAuthTokenProvider _tokenProvider;

        public ApiClient(HttpClient httpClient, IAuthTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
        }

        private void AttachToken()
        {
            var token = _tokenProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// If a response comes back as 401, try to acquire/refresh the token once and retry.
        /// This handles the case where the WebApi wasn't running when the app started.
        /// </summary>
        private async Task<HttpResponseMessage> SendWithRetryAsync(Func<CancellationToken, Task<HttpResponseMessage>> send, CancellationToken cancellationToken = default)
        {
            AttachToken();
            HttpResponseMessage response = await send(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenProvider.RefreshAsync();
                AttachToken();
                response = await send(cancellationToken);
            }
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            }

            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<T?> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
        {
            var token = _tokenProvider.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            AttachToken();
            var response = await _httpClient.GetAsync(uri, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenProvider.RefreshAsync();
                AttachToken();
                response = await _httpClient.GetAsync(uri, cancellationToken);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return default;

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(DeserializeOptions, cancellationToken);
        }

        public async Task PostAsync<TValue>(string uri, TValue value, CancellationToken cancellationToken = default)
        {
            await SendWithRetryAsync(ct => _httpClient.PostAsJsonAsync(uri, value, ct), cancellationToken);
        }

        public async Task<TResponse?> PostAsync<TValue, TResponse>(string uri, TValue value, CancellationToken cancellationToken = default)
        {
            var response = await SendWithRetryAsync(ct => _httpClient.PostAsJsonAsync(uri, value, ct), cancellationToken);
            return await response.Content.ReadFromJsonAsync<TResponse>(DeserializeOptions, cancellationToken);
        }

        public async Task PutAsync<TValue>(string uri, TValue value, CancellationToken cancellationToken = default)
        {
            await SendWithRetryAsync(ct => _httpClient.PutAsJsonAsync(uri, value, ct), cancellationToken);
        }

        public async Task<TResponse?> PutAsync<TValue, TResponse>(string uri, TValue value, CancellationToken cancellationToken = default)
        {
            var response = await SendWithRetryAsync(ct => _httpClient.PutAsJsonAsync(uri, value, ct), cancellationToken);
            return await response.Content.ReadFromJsonAsync<TResponse>(DeserializeOptions, cancellationToken);
        }

        public async Task DeleteAsync(string uri, CancellationToken cancellationToken = default)
        {
            await SendWithRetryAsync(ct => _httpClient.DeleteAsync(uri, ct), cancellationToken);
        }
    }
}
