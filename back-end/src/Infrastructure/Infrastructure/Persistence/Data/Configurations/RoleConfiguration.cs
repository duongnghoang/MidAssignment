﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasMaxLength(20);

        builder.HasData(new List<Role>
        {
            new() { Id = 1, Name = "SUPER_USER" },
            new() { Id = 2, Name = "NORMAL_USER" }
        });
    }
}