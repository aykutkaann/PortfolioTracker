using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IUserRepository
    {

        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct =default);
        Task UpdateAsync(User user, CancellationToken ct = default);
    }
}
