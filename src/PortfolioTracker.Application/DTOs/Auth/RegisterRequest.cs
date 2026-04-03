using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Auth
{
    public record RegisterRequest(
        string Email,
        string Password,
        string FullName);
}
