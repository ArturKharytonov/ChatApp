using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;

namespace ChatApp.UI.Services.HttpClientPWAService
{
    public class HttpClientPwa : IHttpClientPwa
    {
        public const string LoginUrl = "https://localhost:7223/auth/login";
        public const string RegisterUrl = "https://localhost:7223/auth/register";
        public const string ChangePasswordUrl = "https://localhost:7223/auth/change_password";
        public const string GetUser = "https://localhost:7223/api/user";
        public const string UpdateUserCredentials = "https://localhost:7223/api/user/credentials";
        public const string GetUsersBySearch = "https://localhost:7223/api/user/page?";
        public const string GetRoomsBySearch = "https://localhost:7223/api/rooms/page?";
        public const string GetMessagesBySearch = "https://localhost:7223/api/message/page?";
        public const string GetAllMessagesFromChat = "https://localhost:7223/api/message/all/";
        public const string CreateRoom = "https://localhost:7223/api/rooms/creating?";
        public const string GetRoom = "https://localhost:7223/api/rooms?";
        public const string GetUserRooms = "https://localhost:7223/api/user/rooms";
        public const string AddUserToRoom = "https://localhost:7223/api/user";
        public const string Message = "https://localhost:7223/api/message";

        private HttpClient HttpClient { get; set; }
        public HttpClientPwa(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data)
        {
            var result = await HttpClient.PostAsJsonAsync(requestUrl, data);

            return (result.StatusCode.Equals(HttpStatusCode.Unauthorized) && !result.IsSuccessStatusCode)
                ? new ApiRequestResult<VResult>
                {
                    Success = result.IsSuccessStatusCode,
                    Result = default,
                    StatusCode = result.StatusCode
                }
                : new ApiRequestResult<VResult>
                {
                    Success = result.IsSuccessStatusCode,
                    Result = await result.Content.ReadFromJsonAsync<VResult>(),
                    StatusCode = result.StatusCode
                };
        }

        public async Task<ApiRequestResult<VResult>> PutAsync<TArgument, VResult>(string requestUrl, TArgument data)
        {
            var result = await HttpClient.PutAsJsonAsync(requestUrl, data);

            return (result.StatusCode.Equals(HttpStatusCode.Unauthorized) && !result.IsSuccessStatusCode)
                ? new ApiRequestResult<VResult>
                {
                    Success = result.IsSuccessStatusCode,
                    Result = default,
                    StatusCode = result.StatusCode
                }
                : new ApiRequestResult<VResult>
                {
                    Success = result.IsSuccessStatusCode,
                    Result = await result.Content.ReadFromJsonAsync<VResult>(),
                    StatusCode = result.StatusCode
                };
        }


        public async Task<ApiRequestResult<T>> GetAsync<T>(string requestUrl)
        {
            var response = await HttpClient.GetAsync(requestUrl);

            return response.StatusCode.Equals(HttpStatusCode.Unauthorized) && !response.IsSuccessStatusCode
                ? new ApiRequestResult<T>
                {
                    Success = response.IsSuccessStatusCode,
                    Result = default,
                    StatusCode = response.StatusCode
                }
                : new ApiRequestResult<T>
                {
                    Success = response.IsSuccessStatusCode,
                    Result = await response.Content.ReadFromJsonAsync<T>(),
                    StatusCode = response.StatusCode
                };
        }
        public async Task DeleteAsync(string url)
        {
            await HttpClient.DeleteAsync(url);
        }


        public void TryAddJwtToken(string token)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        public void DeleteJwtToken()
        {
            HttpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
