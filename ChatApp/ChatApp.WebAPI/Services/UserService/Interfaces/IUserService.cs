namespace ChatApp.WebAPI.Services.UserService.Interfaces
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(string id, string newPassword, string currentPassword);
    }
}
