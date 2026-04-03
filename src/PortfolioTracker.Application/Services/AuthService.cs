using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PortfolioTracker.Application.DTOs.Auth;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;


namespace PortfolioTracker.Application.Services
{
    public class AuthService(IUserRepository userRepository, IConfiguration configuration, IJwtTokenService jwtTokenService) : IAuthService
    {
        
        
        
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            var existing = await userRepository.GetByEmailAsync(request.Email, ct);
            if (existing != null)
                throw new InvalidOperationException("Email alredy in use.");

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User(
                request.Email,
                hashPassword,
                request.FullName
                );

            var (refreshToken, expiry) =jwtTokenService.GenerateRefreshToken();
            user.UpdateRefreshToken(refreshToken, expiry);

            await userRepository.AddAsync(user, ct);

            var accessToken = jwtTokenService.GenerateJwtToken(user);

            var email = user.Email;

            return new AuthResponse(accessToken, refreshToken, expiry, email);

        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var email = user.Email;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))

                throw new UnauthorizedAccessException("Invalid email or password.");

            var (refreshToken, expiry) = jwtTokenService.GenerateRefreshToken();
            user.UpdateRefreshToken(refreshToken, expiry);
            await userRepository.UpdateAsync(user, ct);    


            var accessToken = jwtTokenService.GenerateJwtToken(user);

            return new AuthResponse(accessToken, refreshToken, expiry, email);

        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct)
        {
            var user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);
            if (user == null)
                throw new Exception("Invalid RefreshToken.");

            var email = user.Email;

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new Exception("Token has been expired.");


            var (refreshToken, expiry) = jwtTokenService.GenerateRefreshToken();

            user.UpdateRefreshToken(refreshToken, expiry);

            await userRepository.UpdateAsync(user, ct);


          

            var accessToken = jwtTokenService.GenerateJwtToken(user);

            return new AuthResponse(accessToken, refreshToken, expiry, email);


        }


    
        
      
    }
}
