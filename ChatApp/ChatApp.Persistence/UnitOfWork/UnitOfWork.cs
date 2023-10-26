using Persistence.User.Interfaces;
using Persistence.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistence.Common.Interfaces;
using Persistence.UnitOfWork.Interfaces;

namespace Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IUserRepository _userRepository => new UserRepository(_context);
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<T>? GetRepository<T>()
            where T : class
        {
            return typeof(T) switch
            {
                { } user when user == typeof(ChatApp.Domain.Users.User) => _userRepository as IRepository<T>,
                _ => throw new ArgumentException($"No repository found for type {typeof(T).Name}")
            };
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
