using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Application.HttpClientPWAService.Interfaces;

namespace ChatApp.Infrastructure.HttpClientPWAService
{
    public class HttpClientPwa : IHttpClientPwa
    {
        private readonly HttpClient _httpClient;

        public HttpClientPwa(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetAuthorizationHeader(AuthenticationHeaderValue? header = null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = header;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await _httpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return await _httpClient.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value)
        {
            return await _httpClient.PostAsJsonAsync(requestUri, value);
        }
    }
}
