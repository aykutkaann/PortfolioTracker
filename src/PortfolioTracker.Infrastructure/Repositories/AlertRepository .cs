using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Repositories
{
    public class AlertRepository( AppDbContext context) :IAlertRepository
    {
        public async Task<List<PriceAlert>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await context.PriceAlerts
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsTriggered)
                .ToListAsync(ct);
                
        }

        public async Task<List<PriceAlert>> GetActiveBySymbolAsync(string symbol, CancellationToken ct)
        {
            var upperSymbol = symbol.ToUpper();

            return await context.PriceAlerts
                .Where(x => x.Symbol == upperSymbol && !x.IsTriggered)
                .ToListAsync(ct);
        }

        public async Task<PriceAlert?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await context.PriceAlerts
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(PriceAlert alert, CancellationToken ct)
        {
            await context.PriceAlerts.AddAsync(alert, ct);
            await context.SaveChangesAsync(ct);

        }

        public async Task UpdateAsync(PriceAlert alert, CancellationToken ct)
        {
            context.PriceAlerts.Update(alert);

            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(PriceAlert alert, CancellationToken ct)
        {
            context.PriceAlerts.Remove(alert);

            await context.SaveChangesAsync(ct);
        }
    }
}
