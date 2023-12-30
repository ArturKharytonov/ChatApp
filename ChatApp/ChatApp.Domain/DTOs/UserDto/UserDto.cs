namespace ChatApp.Domain.DTOs.UserDto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
