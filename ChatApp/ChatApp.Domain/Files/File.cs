using ChatApp.Domain.Common;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;

namespace ChatApp.Domain.Files;

public class File : IDbEntity<string>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public Room Group { get; set; }
    public User User { get; set; }
}