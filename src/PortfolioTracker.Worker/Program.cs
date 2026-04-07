using MassTransit;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Infrastructure.Caching;
using PortfolioTracker.Infrastructure.ExternalApis;
using PortfolioTracker.Infrastructure.Persistence;
using PortfolioTracker.Infrastructure.Repositories;
using PortfolioTracker.Worker;
using PortfolioTracker.Worker.Jobs;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddHttpClient<IPriceClient, CoinGeckoClient>();
builder.Services.AddHostedService<PriceFetchJob>();
builder.Services.AddHostedService<AlertCheckJob>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAlertRepository, AlertRepository>();


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

    });
});

var host = builder.Build();
host.Run();
