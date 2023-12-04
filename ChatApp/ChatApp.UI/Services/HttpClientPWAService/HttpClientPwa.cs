using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;

namespace ChatApp.UI.Services.HttpClientPWAService
{
    public class HttpClientPwa : IHttpClientPwa
    {
        public static string LoginUrl => GetApiUrl("/auth/login");
        public static string RegisterUrl => GetApiUrl("/auth/register");
        public static string ChangePasswordUrl => GetApiUrl("/auth/change_password");
        public static string GetUser => GetApiUrl("/api/user");
        public static string UpdateUserCredentials => GetApiUrl("/api/user/credentials");
        public static string GetUsersBySearch => GetApiUrl("/api/user/page?");
        public static string GetRoomsBySearch => GetApiUrl("/api/rooms/page?");
        public static string GetMessagesBySearch => GetApiUrl("/api/message/page?");
        public static string GetAllMessagesFromChat => GetApiUrl("/api/message/all/");
        public static string CreateRoom => GetApiUrl("/api/rooms/creating?");
        public static string GetRoom => GetApiUrl("/api/rooms?");
        public static string GetUserRooms => GetApiUrl("/api/user/rooms");
        public static string AddUserToRoom => GetApiUrl("/api/user");
        public static string Message => GetApiUrl("/api/message");

        private static string GetApiUrl(string endpoint)
        {
            var apiUrl = Environment.GetEnvironmentVariable("API_URL"); 

            return $"{apiUrl}{endpoint}";
        }

        private HttpClient HttpClient { get; set; }
        public HttpClientPwa(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data)
        {
            var result = await HttpClient.PostAsJsonAsync(requestUrl, data);

            return result.StatusCode.Equals(HttpStatusCode.Unauthorized) && !result.IsSuccessStatusCode
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
