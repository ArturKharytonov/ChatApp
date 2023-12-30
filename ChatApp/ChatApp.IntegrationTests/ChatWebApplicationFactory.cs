using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Context;
using ChatApp.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChatApp.IntegrationTests;

public class ChatWebApplicationFactory : WebApplicationFactory<Program>
{
    private UserManager<User> _userManager;
    private ChatDbContext _context;
    public readonly string ConnectionString = "Data Source=DESKTOP-V85MLET;Database=chat_app_db_test;Trusted_Connection=True; TrustServerCertificate=True";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ChatDbContext>));
            services.AddSqlServer<ChatDbContext>(ConnectionString);

            var serviceProvider = services.BuildServiceProvider();
            _userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            _context = CreateDbContext(services);
            _context.Database.EnsureDeleted();
            if (_context.Database.EnsureCreated())
                AddDataAsync().Wait();
        });
    }

    private async Task AddDataAsync()
    {
        var users = GetTestUsersList();
        foreach (var user in users)
            await _userManager.CreateAsync(user, $"{user.UserName}123!");

        // Create a message with valid RoomId and SenderId
        var messageToDelete = new Message
        {
            Content = "Hello, this is a message to delete.",
            SentAt = DateTime.Now,
            SenderId = users[2].Id
        };
        var messageToUpdate = new Message
        {
            Content = "Hello world.",
            SentAt = DateTime.Now,
            SenderId = users[2].Id
        };

        // Create a room
        var rooms = GetTestRoomsList();
        foreach (var room in rooms)
            await _context.Rooms.AddAsync(room);

        rooms[0].Messages.Add(messageToDelete);
        rooms[0].Messages.Add(messageToUpdate);

        await _context.SaveChangesAsync();
    }

    private static List<User> GetTestUsersList()
    {
        return new List<User>()
        {
            new() { UserName = "loginUser", Email = "user1@example.com" },
            new() { UserName = "changePasswordUser", Email = "user2@example.com" },
            new() { UserName = "UserAndMessages", Email = "user3@example.com" },
            new() { UserName = "UserAndRooms", Email = "user4@example.com"},
            new() { UserName = "UserAndCredentials", Email = "user5@example.com" }
        };
    }

    private static List<Room> GetTestRoomsList()
    {
        return new List<Room>
        {
            new() { Name = "roomForMessages", AssistantId = "123", CreatorId = 123},
            new() { Name = "roomForGetting",AssistantId = "123",  CreatorId = 123},
            new() { Name = "roomForParticipants", AssistantId = "123", CreatorId = 123}
        };
    }

    private static ChatDbContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        return dbContext;
    }
}