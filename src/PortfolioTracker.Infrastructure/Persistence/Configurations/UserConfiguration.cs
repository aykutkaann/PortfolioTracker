using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(h => h.Id).ValueGeneratedNever();


            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);

            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);

            builder.Property(u => u.CreatedAt).HasColumnType("timestamptz");

            builder.Property(u => u.RefreshToken).IsRequired(false).HasMaxLength(255);

            builder.Property(u => u.RefreshTokenExpiryTime).HasColumnType("timestamptz");


            builder.HasMany(u => u.Portfolios)
                .WithOne(p => p.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Watchlists)
                .WithOne(w => w.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PriceAlerts)
                .WithOne(a => a.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
