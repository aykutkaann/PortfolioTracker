using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
    }

   
}
