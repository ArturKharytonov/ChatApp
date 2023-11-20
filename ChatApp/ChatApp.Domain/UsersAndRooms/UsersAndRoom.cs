using ChatApp.Domain.Common;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;

namespace ChatApp.Domain.UsersAndRooms;

public class UsersAndRoom : IDbEntity<(int UserId, int RoomId)>
{
    public (int UserId, int RoomId) Id
    {
        get => (UserId, RoomId);
        set => (UserId, RoomId) = value;
    }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
