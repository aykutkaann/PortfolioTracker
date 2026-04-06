using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;


namespace PortfolioTracker.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration :IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(h => h.Id).ValueGeneratedNever();


            builder.Property(t => t.Type).HasConversion<string>();

            builder.Property(t => t.Quantity).IsRequired().HasPrecision(18, 8);

            builder.Property(t => t.PricePerUnit).IsRequired().HasPrecision(18, 8);

            builder.Property(t => t.ExecutedAt).HasColumnType("timestamptz");

            

            

        }
    }
}
