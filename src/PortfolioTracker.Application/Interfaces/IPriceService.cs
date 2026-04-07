using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IPriceService
    {
        Task<decimal?> GetPriceAsync(string symbol, AssetType assetType);

    }
}
