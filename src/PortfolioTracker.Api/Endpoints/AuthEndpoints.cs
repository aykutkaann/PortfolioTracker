
using FluentValidation;
using PortfolioTracker.Api.Filters;
using PortfolioTracker.Application.DTOs.Auth;
using PortfolioTracker.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PortfolioTracker.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/auth").WithTags("Auths");

            group.MapPost("/register", async (RegisterRequest request, IAuthService service, CancellationToken ct) =>
            {
                try
                {
                    var result = await service.RegisterAsync(request, ct);
                    return Results.Created($"/api/auth/{request.Email}", result);
                }
                catch (InvalidOperationException err)
                {
                    return Results.Conflict(new { message = err.Message });
                }
                catch (Exception err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }

            }).AddEndpointFilter<ValidationFilter<RegisterRequest>>();

            group.MapPost("/login", async (LoginRequest request, IAuthService service, CancellationToken ct) =>
            {

                try
                {
                    var result = await service.LoginAsync(request, ct);
                    return Results.Ok(result);

                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
                catch (Exception err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }


            }).AddEndpointFilter<ValidationFilter<LoginRequest>>();

            group.MapPost("/refresh", async (RefreshTokenRequest request, IAuthService service, CancellationToken ct) =>
            {
                try
                {
                    var refresh = await service.RefreshTokenAsync(request, ct);
                    return Results.Ok(refresh);
                }
                catch (Exception )
                {
                    return Results.Unauthorized();
                }
            });

        }
    }
}
