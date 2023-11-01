namespace ChatApp.WebAPI.Services.JwtHandler.Interfaces
{
    public interface IJwtService
    {
        string GetToken(string username);
    }
}
