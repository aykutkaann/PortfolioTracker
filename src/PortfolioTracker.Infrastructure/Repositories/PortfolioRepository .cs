using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Repositories
{
    public class PortfolioRepository(AppDbContext context) : IPortfolioRepository
    {
        public async Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await context.Portfolios
                .Include(p=> p.Holdings).ThenInclude(h => h.Transactions)
                .FirstOrDefaultAsync(p => p.Id == id,ct);

           

        }

        public async Task<List<Portfolio>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await context.Portfolios
                    .Where(p => p.UserId == userId)
                     .Include(p => p.Holdings).ThenInclude(h => h.Transactions)
                    .ToListAsync(ct);

        }

        public async Task AddAsync(Portfolio portfolio, CancellationToken ct)
        {
            await context.AddAsync(portfolio);

            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Portfolio portfolio, CancellationToken ct)
        {
            await context.SaveChangesAsync(ct);
        }


        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await context.SaveChangesAsync(ct);
        }


        public async Task DeleteAsync(Portfolio portfolio, CancellationToken ct)
        {
            context.Remove(portfolio);

            await context.SaveChangesAsync(ct);
        }
    }
}
