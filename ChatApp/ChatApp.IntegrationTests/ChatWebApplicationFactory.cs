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
    private const string _connectionString = "Server = (localdb)\\mssqllocaldb; Database = chat_app_db_test; Trusted_Connection = True; MultipleActiveResultSets=True;";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ChatDbContext>));
            services.AddSqlServer<ChatDbContext>(_connectionString);

            var serviceProvider = services.BuildServiceProvider();
            _userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            _context = CreateDbContext(services);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            AddDataAsync().Wait();
        });
    }
    private async Task AddDataAsync()
    {
        var loginUser = new User { UserName = "loginUser", Email = "user1@example.com" };
        var changePasswordUser = new User { UserName = "changePasswordUser", Email = "user2@example.com" };
        var userAndMessages = new User { UserName = "UserAndMessages", Email = "user3@example.com" };
        var userAndRooms = new User { UserName = "UserAndRooms", Email = "user4@example.com"};
        var userForUpdatingCredentials = new User { UserName = "UserAndCredentials", Email = "user5@example.com" };

        // Create users without setting Id explicitly
        await _userManager.CreateAsync(loginUser, "Login123!");
        await _userManager.CreateAsync(changePasswordUser, "ChangePassword123!");
        await _userManager.CreateAsync(userAndMessages, "UserAndMessages123!");
        await _userManager.CreateAsync(userAndRooms, "UserAndRooms123!");
        await _userManager.CreateAsync(userForUpdatingCredentials, "UserAndCredentials123!");

        // Create a message with valid RoomId and SenderId
        var messageToDelete = new Message
        {
            Content = "Hello, this is a message to delete.",
            SentAt = DateTime.Now,
            SenderId = userAndMessages.Id
        };
        var messageToUpdate = new Message
        {
            Content = "Hello world.",
            SentAt = DateTime.Now,
            SenderId = userAndMessages.Id
        };

        // Create a room
        var roomForMessages = new Room { Name = "roomForMessages", Messages = { messageToDelete , messageToUpdate} };
        var roomForGetting = new Room { Name = "roomForGetting" };
        var roomForParticipants = new Room { Name = "roomForParticipants" };

        await _context.Rooms.AddAsync(roomForMessages);
        await _context.Rooms.AddAsync(roomForGetting);
        await _context.Rooms.AddAsync(roomForParticipants);

        await _context.SaveChangesAsync();
    }
    private static ChatDbContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        return dbContext;
    }
}