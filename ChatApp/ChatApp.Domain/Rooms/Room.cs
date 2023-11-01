using ChatApp.Domain.Messages;
using ChatApp.Domain.UsersAndRooms;

namespace ChatApp.Domain.Rooms;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<UsersAndRoom> UsersAndRooms { get; set; } = new List<UsersAndRoom>();
}
