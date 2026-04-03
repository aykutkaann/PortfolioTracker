using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }


        public ICollection<Portfolio> Portfolios { get; private set; } = new HashSet<Portfolio>();
        public ICollection<Watchlist> Watchlists { get; private set; } = new HashSet<Watchlist>();
        public ICollection<PriceAlert> PriceAlerts { get; private set; } = new HashSet<PriceAlert>();

        private User() { }

        public User(string email, string passwordHash, string fullName)
        {
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            FullName = fullName;
        }

        public void UpdateRefreshToken(string? token, DateTime? expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiry;

        }
    }

    
}
