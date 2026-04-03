using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Auth
{
    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        string Email);
    
    
}
