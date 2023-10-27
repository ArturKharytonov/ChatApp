using ChatApp.Domain.Rooms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.UsersAndRooms;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations
{
    internal class UserAndRoomConfiguration : IEntityTypeConfiguration<UsersAndRoom>
    {
        public void Configure(EntityTypeBuilder<UsersAndRoom> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__UsersAnd__3214EC0765C4D749");

            builder.HasOne(d => d.Room).WithMany(p => p.UsersAndRooms)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomId_UsersAndRooms");

            builder.HasOne(d => d.User).WithMany(p => p.UsersAndRooms)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId_UsersAndRooms");
        }
    }
}
