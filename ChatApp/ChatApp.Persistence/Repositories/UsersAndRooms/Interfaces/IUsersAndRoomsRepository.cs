using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.UsersAndRooms;
using ChatApp.Persistence.Common.Interfaces;

namespace ChatApp.Persistence.Repositories.UsersAndRooms.Interfaces
{
    public interface IUsersAndRoomsRepository : IRepository<UsersAndRoom>
    {
    }
}
