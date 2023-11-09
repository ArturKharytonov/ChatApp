﻿using ChatApp.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations
{
    internal class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Rooms__3214EC07511992BF");

            builder.Property(e => e.Name).HasMaxLength(30);

            builder.HasMany(users => users.Users)
                .WithMany(users => users.Rooms);
        }
    }
}
