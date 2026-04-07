using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IPriceClient
    {
        Task<Dictionary<string, decimal>> GetCryptoPricesAsync(IEnumerable<string> symbols);
        Task<decimal?> GetStockPriceAsync(string symbol);

    }
}
