namespace ChatApp.Domain.DTOs.Http.Responses.Users
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? Token { get; set; }
    }
}
