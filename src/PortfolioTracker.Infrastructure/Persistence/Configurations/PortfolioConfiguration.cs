using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Persistence.Configurations
{
    public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
    {

        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            builder.ToTable("Portfolios");

            builder.HasKey(p => p.Id);

            builder.Property(h => h.Id).ValueGeneratedNever();


            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);

            builder.Property(p => p.CreatedAt).HasColumnType("timestamptz");


        

            builder.HasMany(p => p.Holdings)
                .WithOne(h => h.Portfolio)
                .HasForeignKey(p => p.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
