namespace ChatApp.Domain.DTOs.Http.Responses.Files;

public class FilesFromUserResponseDto
{
    public List<FileDto.FileDto> Files { get; set; } = new();
}