﻿using ChatApp.Domain.Common;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Users;
using ChatApp.Domain.UsersAndRooms;

namespace ChatApp.Domain.Rooms;

public class Room : IDbEntity<int>
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
