using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Domain.Entities
{
    public class Portfolio
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User User { get; private set; }

        public ICollection<Holding> Holdings { get; private set; } = new HashSet<Holding>();


    }
}
