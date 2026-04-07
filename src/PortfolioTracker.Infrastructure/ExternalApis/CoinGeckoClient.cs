using PortfolioTracker.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PortfolioTracker.Infrastructure.ExternalApis
{
    public class CoinGeckoClient :IPriceClient
    {
        private readonly HttpClient _client;

        public CoinGeckoClient(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("PortfolioTracker/1.0");

        }


        private static readonly Dictionary<string, string> SymbolToIdMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "BTC", "bitcoin" },
            { "ETH", "ethereum" },
            { "SOL", "solana" },
            { "ADA", "cardano" },
            { "DOT", "polkadot" },
            { "AVAX", "avalanche-2" }

        };

        public async Task<Dictionary<string, decimal>> GetCryptoPricesAsync(IEnumerable<string> symbols)
        {

            var ids = symbols
                .Select(s => SymbolToIdMap.TryGetValue(s, out var id) ? id : null)
                .Where(id => id != null);

            if (!ids.Any()) return new Dictionary<string, decimal>();

            var idList = string.Join(",", ids);
            var url = $"https://api.coingecko.com/api/v3/simple/price?ids={idList}&vs_currencies=usd";

            var response = await _client.GetFromJsonAsync<Dictionary<string, Dictionary<string, decimal>>>(url);

            var result = new Dictionary<string, decimal>();
            foreach (var symbol in symbols)
            {
                if (SymbolToIdMap.TryGetValue(symbol, out var id) && response.TryGetValue(id, out var priceInfo))
                {
                    result[symbol.ToUpper()] = priceInfo["usd"];
                }
            }

            return result;
        }

        public async Task<decimal?> GetStockPriceAsync(string symbol)
        {
            return null;
        }
    }
}
