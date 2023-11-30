using ChatApp.Domain.Common;
using ChatApp.Domain.Friends;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.UsersAndRooms;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Users;

public class User : IdentityUser<int>, IDbEntity<int>
{
    public ICollection<Friend> FriendFirstUsers { get; set; } = new List<Friend>();

    public ICollection<Friend> FriendSecondUsers { get; set; } = new List<Friend>();

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
