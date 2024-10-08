namespace ChatApp.Domain.DTOs.Http.Requests.Users;

public class AddUserToRoomDto
{
    public string UserId { get; set; }
    public string RoomId { get; set; }
}