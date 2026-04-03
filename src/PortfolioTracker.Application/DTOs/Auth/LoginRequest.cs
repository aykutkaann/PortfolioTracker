using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Auth
{
    public record LoginRequest(
        string Email,
        string Password);
    
}
