using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PortfolioTracker.Application.Services
{
    public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
    {
        public (string Token, DateTime Expiry) GenerateRefreshToken()
        {
            var randomNumer = new byte[32];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumer);

            return (Token: Convert.ToBase64String(randomNumer),
                Expiry: DateTime.UtcNow.AddDays(7));
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
