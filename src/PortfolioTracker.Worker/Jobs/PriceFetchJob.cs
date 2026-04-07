using PortfolioTracker.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Worker.Jobs
{

    public class PriceFetchJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PriceFetchJob> _logger;
        private readonly List<string> _symbols = new() { "BTC", "ETH", "SOL" };

        public PriceFetchJob(IServiceScopeFactory scopeFactory, ILogger<PriceFetchJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                    var priceClient = scope.ServiceProvider.GetRequiredService<IPriceClient>();

                    var prices = await priceClient.GetCryptoPricesAsync(_symbols);


                    foreach (var (symbol, price) in prices)
                    {
                        var cacheKey = $"price:{symbol.ToUpper()}";
                        await cacheService.SetAsync(cacheKey, price, TimeSpan.FromSeconds(60));
                    }

                    _logger.LogInformation("{Time}: {Count} crypto prices has been updated",
                        DateTimeOffset.Now, prices.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while fetching prices.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
