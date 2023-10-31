using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.Rooms;
using ChatApp.Persistence.Common.Interfaces;

namespace ChatApp.Persistence.Repositories.Rooms.Interfaces
{
    public interface IRoomRepository : IRepository<Room>
    {
    }
}
