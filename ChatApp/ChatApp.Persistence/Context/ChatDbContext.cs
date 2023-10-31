using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.UsersAndRooms;
using ChatApp.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Context;

public partial class ChatDbContext : DbContext
{
    public ChatDbContext()
    {
    }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options) { }

    public virtual DbSet<Domain.Friends.Friend> Friends { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Domain.Users.User> Users { get; set; }

    public virtual DbSet<UsersAndRoom> UsersAndRooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new FriendConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new UserAndRoomConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
