using PortfolioTracker.Application.DTOs.Alert;
using PortfolioTracker.Application.Interfaces;

namespace PortfolioTracker.Api.Endpoints
{
    public static class AlertEndpoints
    {

        public static void MapAlertEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/alerts").WithTags("Alerts").RequireAuthorization();

            group.MapPost("/", async (CreateAlertRequest request , IAlertService service, CancellationToken ct) =>
            {
                try
                {
                    var result = await service.CreateAsync(request, ct);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException err)
                {
                    return Results.Conflict(new { message = err.Message });
                }
                catch (Exception err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }


            }).RequireAuthorization();


            group.MapGet("/", async (IAlertService service, CancellationToken ct) =>
            {
                return await service.GetMyAlertsAsync(ct);
            }).RequireAuthorization();

            group.MapDelete("/{id:guid}", async (Guid id, IAlertService service, CancellationToken ct) =>
            {

                await service.DeleteAsync(id, ct);

            }).RequireAuthorization();


        }
    }
}
