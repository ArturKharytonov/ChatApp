namespace ChatApp.Domain.DTOs.Http.Responses;

public class FilesFromUserResponseDto
{
    public List<FileDto.FileDto> Files { get; set; } = new();
}