using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Domain.Entities
{
    public class Holding
    {

        public Guid Id { get; private set; }
        public Guid PortfolioId { get; private set; }
        public string Symbol { get; private set; }
        public AssetType AssetType { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal AverageBuyPrice { get; private set; }


        public Portfolio Portfolio { get; private set; }

        public ICollection<Transaction> Transactions { get; private set; } = new HashSet<Transaction>();


        private Holding () { }

        public Holding(Guid portfolioId, string symbol, AssetType assetType, decimal quantity, decimal price)
        {
            Id = Guid.NewGuid();
            PortfolioId = portfolioId;
            Symbol = symbol;
            AssetType = assetType;
            Quantity = quantity;
            AverageBuyPrice = price;
        }

        public void AddQuantity(decimal quantity, decimal price)
        {
            AverageBuyPrice = ((Quantity * AverageBuyPrice) + (quantity * price)) / (Quantity + quantity);
            Quantity += quantity;

        }

        public void RemoveQuantity(decimal quantity)
        {
            if (quantity > Quantity)
                throw new InvalidOperationException("Insufficient quantity.");
            Quantity -= quantity;
        }
    }
}
