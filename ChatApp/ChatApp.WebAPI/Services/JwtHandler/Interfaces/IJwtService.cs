namespace ChatApp.WebAPI.Services.JwtHandler.Interfaces
{
    public interface IJwtService
    {
        string GetToken(int id, string username);
    }
}
