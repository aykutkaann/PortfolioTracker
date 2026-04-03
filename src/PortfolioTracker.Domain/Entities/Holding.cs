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
    }
}
