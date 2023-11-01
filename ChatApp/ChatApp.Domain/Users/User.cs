using ChatApp.Domain.Friends;
using ChatApp.Domain.Messages;
using ChatApp.Domain.UsersAndRooms;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Users;

public class User : IdentityUser<int>
{
    public virtual ICollection<Friend> FriendFirstUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendSecondUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<UsersAndRoom> UsersAndRooms { get; set; } = new List<UsersAndRoom>();
}
