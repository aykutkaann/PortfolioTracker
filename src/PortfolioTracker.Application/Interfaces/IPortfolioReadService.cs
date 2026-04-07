using PortfolioTracker.Application.DTOs.Portfolio;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IPortfolioReadService
    {
        Task<List<PortfolioDashboardDto>> GetDashboardAsync(CancellationToken ct);
        Task<List<HoldingDetailDto>> GetHoldingDetailsAsync(Guid portfolioId, CancellationToken ct);
    }
}
