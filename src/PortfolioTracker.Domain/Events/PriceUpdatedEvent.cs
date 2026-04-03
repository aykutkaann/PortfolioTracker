using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Domain.Events
{
    public record PriceUpdatedEvent(
        string Symbol,
        AssetType AssetType,
        decimal Price,
        DateTime Timestamp);
}
