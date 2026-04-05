using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<Portfolio>> GetByUserIdAsync(Guid userId, CancellationToken ct);
        Task AddAsync(Portfolio portfolio, CancellationToken ct);
        Task UpdateAsync(Portfolio portfolio, CancellationToken ct);
        Task DeleteAsync(Portfolio portfolio, CancellationToken ct);
    }
}
