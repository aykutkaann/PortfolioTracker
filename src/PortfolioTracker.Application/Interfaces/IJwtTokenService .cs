using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IJwtTokenService
    {
        (string Token, DateTime Expiry) GenerateRefreshToken();
        string GenerateJwtToken(User user);
    }
}
