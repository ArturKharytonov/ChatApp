using Microsoft.EntityFrameworkCore;
using ChatApp.Domain.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Messages__3214EC0707BA7D0D");

            builder.Property(m => m.Content).IsRequired();
            builder.Property(m => m.SentAt).IsRequired();

            builder.HasOne(d => d.Room)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomId_Messages");

            builder.HasOne(d => d.Sender)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SenderId_Messages");
        }
    }
}
