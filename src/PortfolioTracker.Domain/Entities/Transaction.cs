using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid HoldingId { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal PricePerUnit { get; private set; }
        public DateTime ExecutedAt { get; private set; }

        public Holding Holding { get; private set; }

        private Transaction() { }

        public Transaction(Guid holdingId, TransactionType type, decimal quantity, decimal pricePerUnit)
        {
            Id = Guid.NewGuid();
            HoldingId = holdingId;
            Type = type;
            Quantity = quantity;
            PricePerUnit = pricePerUnit;
            ExecutedAt = DateTime.UtcNow;
        }

    }
}
