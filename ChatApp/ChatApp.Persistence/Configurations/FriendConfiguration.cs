using Microsoft.EntityFrameworkCore;
using ChatApp.Domain.Friends;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations
{
    internal class FriendConfiguration : IEntityTypeConfiguration<Friend>
    {
        public void Configure(EntityTypeBuilder<Friend> builder)
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
