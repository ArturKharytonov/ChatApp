using ChatApp.Domain.DTOs.FileDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Rooms;
using File = ChatApp.Domain.Files.File;

namespace ChatApp.Domain.Mappers.Files;

public static class FileMapper
{
    public static FileDto ToFileDto(this File file)
    {
        return new FileDto()
        {
            Id = file.Id,
            FileName = file.Name,
            RoomName = file.Group.Name,
            Username = file.User.UserName
        };
    }
}