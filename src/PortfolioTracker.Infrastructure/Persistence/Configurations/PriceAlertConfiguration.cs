using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Persistence.Configurations
{
    public class PriceAlertConfiguration :IEntityTypeConfiguration<PriceAlert>
    {

        public void Configure(EntityTypeBuilder<PriceAlert> builder)
        {
            builder.ToTable("PriceAlerts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Symbol).IsRequired().HasMaxLength(20);

            builder.Property(p => p.AssetType).HasConversion<string>().IsRequired();

            builder.Property(p => p.TargetPrice).HasPrecision(18, 8).IsRequired();

            builder.Property(p => p.CreatedAt).IsRequired().HasColumnType("timestamptz");

            
        }
    }
}
