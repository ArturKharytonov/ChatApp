using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Domain.UsersAndRooms;
using ChatApp.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Context;

public partial class ChatDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ChatDbContext() { }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options){}


    public virtual DbSet<Domain.Friends.Friend> Friends { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<User> AspNetUsers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FriendConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
