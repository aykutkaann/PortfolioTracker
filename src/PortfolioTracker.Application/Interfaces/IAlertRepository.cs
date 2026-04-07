using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IAlertRepository
    {
        Task<List<PriceAlert>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
        Task<List<PriceAlert>> GetActiveBySymbolAsync(string symbol, CancellationToken ct);
        Task<PriceAlert?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(PriceAlert alert, CancellationToken ct);
        Task UpdateAsync(PriceAlert alert, CancellationToken ct);
        Task DeleteAsync(PriceAlert alert, CancellationToken ct);
    }
}
