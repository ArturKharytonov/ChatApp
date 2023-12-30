using ChatApp.Domain.DTOs.FileDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Mappers.Files;
using ChatApp.Domain.Rooms;

namespace ChatApp.Domain.Mappers.Rooms;

public static class RoomMapper
{
    public static RoomDto ToChatDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            CreatorId = room.CreatorId,
            ParticipantsNumber = room.Users.Count,
            MessagesNumber = room.Messages.Count,
            AssistantId = room.AssistantId,
            Files = room.Files.Select(f => f.ToFileDto()).ToList()
        };
    }
}