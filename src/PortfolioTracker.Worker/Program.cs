using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Infrastructure.Caching;
using PortfolioTracker.Infrastructure.ExternalApis;
using PortfolioTracker.Worker;
using PortfolioTracker.Worker.Jobs;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddHttpClient<IPriceClient, CoinGeckoClient>();
builder.Services.AddHostedService<PriceFetchJob>();

var host = builder.Build();
host.Run();
