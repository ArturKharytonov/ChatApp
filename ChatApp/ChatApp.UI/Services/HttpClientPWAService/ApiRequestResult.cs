using System.Net;

namespace ChatApp.UI.Services.HttpClientPWAService
{
    public class ApiRequestResult<TResult>
    {
        public bool Success { get; set; }
        public TResult? Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
