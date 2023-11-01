using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Persistence.Common.Interfaces;

namespace ChatApp.Persistence.Repositories.Message.Interfaces
{
    public interface IMessageRepository : IRepository<Domain.Messages.Message>
    {
    }
}
