using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;

namespace ChatApp.Domain.UsersAndRooms;

public partial class UsersAndRoom
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
