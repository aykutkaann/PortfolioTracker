using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Application.Services;
using PortfolioTracker.Infrastructure.Persistence;
using PortfolioTracker.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//Services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();



var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
