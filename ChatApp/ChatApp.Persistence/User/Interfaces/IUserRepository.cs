using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence.Common.Interfaces;
namespace Persistence.User.Interfaces
{
    public interface IUserRepository : IRepository<Domain.Users.User>
    {
        // add some additional functional for user
    }
}
