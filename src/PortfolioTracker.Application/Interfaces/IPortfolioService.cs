using PortfolioTracker.Application.DTOs.Portfolio;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IPortfolioService
    {
             Task<PortfolioResponse> CreateAsync(CreatePortfolioRequest request, CancellationToken ct);
             Task<List<PortfolioResponse>> GetAllAsync(CancellationToken ct);
             Task<PortfolioResponse> GetByIdAsync(Guid id, CancellationToken ct);
             Task DeleteAsync(Guid id , CancellationToken ct);
             Task<TransactionResponse> AddTransactionAsync(Guid id, AddTransactionRequest request, CancellationToken ct);

    }
}
