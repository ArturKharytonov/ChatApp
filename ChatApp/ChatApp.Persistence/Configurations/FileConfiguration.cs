using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = ChatApp.Domain.Files.File;

namespace ChatApp.Persistence.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Files__3214EC076A668A68");

        builder.Property(f => f.Id).IsRequired();
        builder.Property(f => f.Name).IsRequired();
        builder.Property(f => f.GroupId).IsRequired();
        builder.Property(f => f.UserId).IsRequired();

        builder
            .HasOne(d => d.Group)
            .WithMany(p => p.Files)
            .HasForeignKey(d => d.GroupId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder
            .HasOne(d => d.User)
            .WithMany(p => p.SentFiles)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}