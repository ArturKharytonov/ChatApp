using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ChatApp.Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<Domain.Users.User>
    {
        public void Configure(EntityTypeBuilder<Domain.Users.User> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Users__3214EC07AEE8155C");

            builder.Property(e => e.Email)
                .HasMaxLength(40)
                .IsUnicode(false);
            builder.Property(e => e.Password)
                .HasMaxLength(30)
                .IsUnicode(false);
            builder.Property(e => e.Username)
                .HasMaxLength(40)
                .IsUnicode(false);
        }
    }
}
