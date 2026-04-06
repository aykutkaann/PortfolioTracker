using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Persistence.Configurations
{
    public class HoldingConfiguration :IEntityTypeConfiguration<Holding>
    {
        public void Configure(EntityTypeBuilder<Holding> builder)
        {
            builder.ToTable("Holdings");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id).ValueGeneratedNever();


            builder.Property(h => h.Symbol).IsRequired().HasMaxLength(20);

            builder.Property(h => h.Quantity).IsRequired().HasPrecision(18, 8);

            builder.Property(h => h.AverageBuyPrice).IsRequired().HasPrecision(18, 8);

            builder.Property(h => h.AssetType).HasConversion<string>().IsRequired();





            builder.HasMany(h => h.Transactions)
                .WithOne(t => t.Holding)
                .HasForeignKey(h => h.HoldingId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
