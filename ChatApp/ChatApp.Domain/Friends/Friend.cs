﻿using ChatApp.Domain.Common;
using ChatApp.Domain.Users;

namespace ChatApp.Domain.Friends;

public class Friend : IDbEntity<int>
{
    public int Id { get; set; }

    public int FirstUserId { get; set; }

    public int SecondUserId { get; set; }

    public virtual User FirstUser { get; set; } = null!;

    public virtual User SecondUser { get; set; } = null!;
}
