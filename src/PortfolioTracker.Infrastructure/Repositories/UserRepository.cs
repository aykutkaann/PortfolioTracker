using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) :IUserRepository
    {

        public async Task<User?> GetByEmailAsync(string email,CancellationToken ct)
        {
            return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct =default)
        {
            await context.AddAsync(user);
            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync(ct);
        }


    }
}
