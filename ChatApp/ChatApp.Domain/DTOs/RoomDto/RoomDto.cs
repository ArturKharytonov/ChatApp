namespace ChatApp.Domain.DTOs.RoomDto;

public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AssistantId { get; set; }
    public int MessagesNumber { get; set; }
    public int ParticipantsNumber { get; set; }
    public int CreatorId { get; set; }
    public IEnumerable<FileDto.FileDto> Files { get; set; }
}