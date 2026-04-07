using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Services
{
    public class PriceService(IPriceClient priceClient, ICacheService cacheService) :IPriceService
    {
        public async Task<decimal?> GetPriceAsync(string symbol, AssetType assetType)
        {
            var cacheKey = $"price:{symbol.ToUpper()}";


            var cachedPrice = await cacheService.GetAsync<decimal?>(cacheKey);
            if (cachedPrice.HasValue)
            {
                return cachedPrice.Value;
            }

            decimal? price = null;
            if (assetType == AssetType.Crypto)
            {
                var prices = await priceClient.GetCryptoPricesAsync(new[] { symbol });
                if (prices.TryGetValue(symbol.ToUpper(), out var p)) price = p;
            }
            else
            {
                price = await priceClient.GetStockPriceAsync(symbol);
            }

            
            if (price.HasValue)
            {
                await cacheService.SetAsync(cacheKey, price.Value, TimeSpan.FromSeconds(60));

            }

            return price;
        }

    }
}
