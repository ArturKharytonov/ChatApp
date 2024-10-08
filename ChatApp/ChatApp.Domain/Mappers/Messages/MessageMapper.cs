using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Messages;

namespace ChatApp.Domain.Mappers.Messages;

public static class MessageMapper
{
    public static MessageDto ToMessageDto(this Message message)
    {
        return new()
        {
            Id = message.Id,
            Content = message.Content,
            RoomName = message.Room.Name,
            SenderUsername = message.Sender.UserName,
            SentAt = message.SentAt
        };
    }
}