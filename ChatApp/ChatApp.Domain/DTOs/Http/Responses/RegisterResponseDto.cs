namespace ChatApp.Domain.DTOs.Http.Responses
{
    public class RegisterResponseDto
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
