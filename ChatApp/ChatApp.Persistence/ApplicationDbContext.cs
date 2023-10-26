using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;

namespace Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ChatApp.Domain.Users.User> Users { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configure using fluentAPI
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}