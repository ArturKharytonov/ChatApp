using ChatApp.Domain.Friends;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Domain.UsersAndRooms;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.Repositories.Friend;
using ChatApp.Persistence.Repositories.Friend.Interfaces;
using ChatApp.Persistence.Repositories.Message;
using ChatApp.Persistence.Repositories.Message.Interfaces;
using ChatApp.Persistence.Repositories.Rooms;
using ChatApp.Persistence.Repositories.Rooms.Interfaces;
using ChatApp.Persistence.Repositories.User;
using ChatApp.Persistence.Repositories.User.Interfaces;
using ChatApp.Persistence.Repositories.UsersAndRooms;
using ChatApp.Persistence.Repositories.UsersAndRooms.Interfaces;
using ChatApp.Persistence.UnitOfWork.Interfaces;

namespace ChatApp.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;
        private IUserRepository UserRepository => new UserRepository(_context);
        private IFriendRepository FriendRepository => new FriendRepository(_context);
        private IMessageRepository MessageRepository => new MessageRepository(_context);
        private IRoomRepository RoomRepository => new RoomRepository(_context);
        private IUsersAndRoomsRepository UsersAndRooms => new UsersAndRoomsRepository(_context);

        public UnitOfWork(ChatDbContext context)
        {
            _context = context;
        }

        public IRepository<T>? GetRepository<T>()
            where T : class
        {
            return typeof(T) switch
            {
                { } user when user == typeof(User) => UserRepository as IRepository<T>,
                { } friend when friend == typeof(Friend) => FriendRepository as IRepository<T>,
                { } message when message == typeof(Message) => MessageRepository as IRepository<T>,
                { } room when room == typeof(Room) => RoomRepository as IRepository<T>,
                { } usersAndRooms when usersAndRooms == typeof(UsersAndRoom) => UsersAndRooms as IRepository<T>,
                _ => throw new ArgumentException($"No repository found for type {typeof(T).Name}")
            };
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
