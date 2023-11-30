using ChatApp.Domain.Common;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;

namespace ChatApp.Domain.Messages;

public class Message : IDbEntity<int>
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public int RoomId { get; set; }

    public int SenderId { get; set; }

    public Room Room { get; set; } = null!;

    public User Sender { get; set; } = null!;
}
