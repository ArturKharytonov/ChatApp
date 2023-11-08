using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;

namespace ChatApp.Domain.Messages;

public class Message
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public byte[] SentAt { get; set; } = null!;

    public int RoomId { get; set; }

    public int SenderId { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
