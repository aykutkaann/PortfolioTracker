using MassTransit;
using MassTransit.Transports;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Events;
using PortfolioTracker.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PortfolioTracker.Worker.Jobs
{
    public class AlertCheckJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AlertCheckJob> _logger;
        private readonly List<string> _symbols = new() { "BTC", "ETH", "SOL" };


        public AlertCheckJob(IServiceScopeFactory scopeFactory, ILogger<AlertCheckJob> logger)
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
                    var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();


                    foreach (var symbol in _symbols)
                    {
                        var price = await cacheService.GetAsync<decimal?>($"price:{symbol}");
                        if (price is null) continue;

                        var alerts = await alertRepository.GetActiveBySymbolAsync(symbol, stoppingToken);

                        foreach (var alert in alerts)
                        {
                            bool triggered = alert.IsAbove
                                ? price >= alert.TargetPrice
                                : price <= alert.TargetPrice;

                            if (!triggered) continue;

                            alert.Trigger();
                            await alertRepository.UpdateAsync(alert, stoppingToken);

                            await publishEndpoint.Publish(new AlertTriggeredEvent(
                                alert.Id, alert.UserId, symbol, price.Value, alert.TargetPrice
                            ), stoppingToken);

                            _logger.LogInformation("Alert triggered for {Symbol} at {Price}", symbol, price);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking alerts");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

}
