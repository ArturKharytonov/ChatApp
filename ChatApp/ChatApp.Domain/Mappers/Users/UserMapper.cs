using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Users;

namespace ChatApp.Domain.Mappers.Users;

public static class UserMapper
{
    public static UserDto ToUserDto(this User user)
    {
        return new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber
        };
    }
}