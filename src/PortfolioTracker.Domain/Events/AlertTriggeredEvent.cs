using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Domain.Events
{
    public record AlertTriggeredEvent(
        Guid AlertId,
        Guid UserId,
        string Symbol,
        decimal CurrentPrice,
        decimal TargetPrice);
}
