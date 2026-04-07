using Microsoft.AspNetCore.Http.HttpResults;
using PortfolioTracker.Application.DTOs.Portfolio;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Application.Services;
using System.Runtime.CompilerServices;

namespace PortfolioTracker.Api.Endpoints
{
    public static class PortfolioEndpoints
    {
        public static void MapPortfolioEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/portfolios").WithTags("Portfolios").RequireAuthorization();

            group.MapGet("/", async (IPortfolioService service, CancellationToken ct) =>
            {

               return await service.GetAllAsync(ct);

            });
            group.MapGet("/dashboard", async (IPortfolioReadService readService, CancellationToken ct) =>
            {
                var result = await readService.GetDashboardAsync(ct);
                return Results.Ok(result);

            });
            group.MapPost("/", async (CreatePortfolioRequest request, IPortfolioService service, CancellationToken ct) =>
            {

                var result = await service.CreateAsync(request, ct);

                return Results.Ok(result);

            });
            group.MapGet("/{id:guid}", async (Guid id, IPortfolioService service,  CancellationToken ct) =>
            {
                var portfolio = await service.GetByIdAsync(id, ct);

                if (portfolio == null) throw new Exception("Portfolio is not found.");
                return Results.Ok(portfolio);


            });
            group.MapGet("/{id:guid}/holdings", async (Guid id, IPortfolioReadService readService, CancellationToken ct) =>
            {
                var result = await readService.GetHoldingDetailsAsync(id, ct) ?? throw new Exception("Holding not found.");

                return Results.Ok(result);
            });
            group.MapDelete("/{id:guid}", async (Guid id, IPortfolioService service, CancellationToken ct) =>
            {

                await service.DeleteAsync(id, ct);

                return Results.NoContent();

            });
            group.MapPost("/{id:guid}/transactions", async (Guid id, AddTransactionRequest request, IPortfolioService service, CancellationToken ct) =>
            {
                var transaction = await service.AddTransactionAsync(id, request, ct);

                return Results.Ok(transaction);

            });





        }
    }
}
