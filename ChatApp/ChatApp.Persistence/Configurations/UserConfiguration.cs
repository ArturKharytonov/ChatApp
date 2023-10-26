using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<ChatApp.Domain.Users.User>
    {
        public void Configure(EntityTypeBuilder<ChatApp.Domain.Users.User> builder)
        {
            // using of Fluent API here
        }
    }
}
