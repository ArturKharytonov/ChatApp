namespace ChatApp.UI.Services.HttpClientPWAService.Interfaces
{
    public interface IHttpClientPwa
    {
        Task<ApiRequestResult<T>> GetAsync<T>(string requestUrl);
        Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data);
        Task<ApiRequestResult<VResult>> PutAsync<TArgument, VResult>(string requestUrl, TArgument data);
        Task DeleteAsync(string url);
        void TryAddJwtToken(string token);
        void DeleteJwtToken();
    }
}
