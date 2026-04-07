using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Alert
{
    public record AlertResponse(Guid Id,
        string Symbol,
        string AssetType,
        decimal TargetPrice,
        bool IsAbove,
        bool IsTrigered,
        DateTime CreatedAt);


}
