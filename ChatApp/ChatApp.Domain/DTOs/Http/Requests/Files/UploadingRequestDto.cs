namespace ChatApp.Domain.DTOs.Http.Requests.Files;

public class UploadingRequestDto
{
    public string FileName { get; set; } = null!;
    public string FileId { get; set; } = null!;
    public string RoomId { get; set; } = null!;
    public string SenderId { get; set; } = null!;
}