using PortfolioTracker.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request,CancellationToken ct);

        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);

        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct);


    }
}
