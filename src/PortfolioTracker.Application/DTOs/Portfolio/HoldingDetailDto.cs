using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Portfolio
{
    public record HoldingDetailDto(
        Guid HoldingId,
        string Symbol,
        string AssetType,
        decimal Quantity,
        decimal AverageBuyPrice,
        decimal CurrentPrice,
        decimal TotalInvested,
        decimal CurrentValue,
        decimal ProfitLoss,
        decimal ProfitLossPercent);
    
    
}
