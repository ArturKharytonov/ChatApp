namespace ChatApp.UI.Services.HttpClientPWAService.Interfaces
{
    public interface IHttpClientPwa
    {
        Task<ApiRequestResult<T>> GetAsync<T>(string requestUrl);
        Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data);
        void TryAddJwtToken(string token);
        void DeleteJwtToken();
    }
}
