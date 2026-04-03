using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Domain.Entities
{
    public class Watchlist
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Symbol { get; private set; }
        public AssetType AssetType { get; private set; }
        public DateTime AddedAt { get; private set; }


        public User User { get;private set; }
    }
}
