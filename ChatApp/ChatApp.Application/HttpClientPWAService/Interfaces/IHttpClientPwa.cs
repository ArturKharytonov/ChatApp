using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.HttpClientPWAService.Interfaces
{
    public interface IHttpClientPwa
    {
        void SetAuthorizationHeader(AuthenticationHeaderValue? header = null);
        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value);
    }
}
