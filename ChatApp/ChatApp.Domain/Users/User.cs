using ChatApp.Domain.Friends;
using ChatApp.Domain.Messages;
using ChatApp.Domain.UsersAndRooms;

namespace ChatApp.Domain.Users;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Friend> FriendFirstUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendSecondUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<UsersAndRoom> UsersAndRooms { get; set; } = new List<UsersAndRoom>();
}
