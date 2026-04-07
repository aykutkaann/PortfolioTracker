using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Portfolio
{
    public record PortfolioDashboardDto(
        Guid PortfolioId,
        string PortfolioName,
        int TotalHoldings,
        decimal TotalInvested,
        decimal CurrentValue,
        decimal ProfitLoss,
        decimal ProfitLossPercent);
    
    
}
