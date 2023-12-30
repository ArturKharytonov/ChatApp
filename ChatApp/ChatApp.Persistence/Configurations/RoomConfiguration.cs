using Microsoft.EntityFrameworkCore;
using ChatApp.Domain.Rooms;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations
{
    internal class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Rooms__3214EC07511992BF");

            builder.Property(e => e.Name).HasMaxLength(30);

            builder.HasMany(x => x.Users)
                .WithMany(x => x.Rooms);
        }
    }
}
