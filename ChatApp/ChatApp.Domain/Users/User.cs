using ChatApp.Domain.Common;
using ChatApp.Domain.Friends;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using Microsoft.AspNetCore.Identity;
using File = ChatApp.Domain.Files.File;

namespace ChatApp.Domain.Users;

public class User : IdentityUser<int>, IDbEntity<int>
{
    public ICollection<Friend> FriendFirstUsers { get; set; } = new List<Friend>();

    public ICollection<Friend> FriendSecondUsers { get; set; } = new List<Friend>();

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<File> SentFiles { get; set; } = new List<File>();
}
