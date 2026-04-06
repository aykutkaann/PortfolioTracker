using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Persistence.Configurations
{
    public class WatchlistConfiguration :IEntityTypeConfiguration<Watchlist>
    {

        public void Configure(EntityTypeBuilder<Watchlist> builder)
        {
            builder.ToTable("Watchlists");

            builder.HasKey(w => w.Id);

            builder.Property(h => h.Id).ValueGeneratedNever();


            builder.Property(w => w.Symbol).IsRequired().HasMaxLength(20);

            builder.Property(w => w.AssetType).HasConversion<string>().IsRequired();

            builder.Property(w => w.AddedAt).HasColumnType("timestamptz");

            
        }
    }
}
