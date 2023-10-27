using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.Friends;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations
{
    internal class FriendConfiguration : IEntityTypeConfiguration<Domain.Friends.Friend>
    {
        public void Configure(EntityTypeBuilder<Domain.Friends.Friend> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Friends__3214EC076A668A68");

            builder.HasOne(d => d.FirstUser).WithMany(p => p.FriendFirstUsers)
                .HasForeignKey(d => d.FirstUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FirstFriend");

            builder.HasOne(d => d.SecondUser).WithMany(p => p.FriendSecondUsers)
                .HasForeignKey(d => d.SecondUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SecondFriend");
        }
    }
}
