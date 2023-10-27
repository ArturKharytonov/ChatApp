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

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = chat_app_db; Trusted_Connection = True;");

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
