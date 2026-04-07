using PortfolioTracker.Application.DTOs.Alert;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Services
{
    public class AlertService(IAlertRepository alertRepository, ICurrentUserService currentUserService) :IAlertService
    {
        public async Task<AlertResponse> CreateAsync(CreateAlertRequest request, CancellationToken ct)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var assetType = Enum.Parse<AssetType>(request.AssetType, ignoreCase: true);

            var alert = new PriceAlert(userId, request.Symbol.ToUpper(), assetType, request.TargetPrice, request.IsAbove);

            await alertRepository.AddAsync(alert, ct);

            return MapToResponse(alert);
       
        }

        public async Task<List<AlertResponse>> GetMyAlertsAsync(CancellationToken ct = default)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var alerts = await alertRepository.GetActiveByUserIdAsync(userId, ct);

            return alerts.Select(MapToResponse).ToList();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var alerts = await alertRepository.GetByIdAsync(id, ct) ?? throw new UnauthorizedAccessException();
            if (alerts.UserId != currentUserService.UserId)
                throw new UnauthorizedAccessException();

            await alertRepository.DeleteAsync(alerts, ct);
        }


        private static AlertResponse MapToResponse(PriceAlert alert) 
        {

            return new AlertResponse(
                alert.Id,
                alert.Symbol,
                alert.AssetType.ToString(),
                alert.TargetPrice,
                alert.IsAbove,
                alert.IsTriggered,
                alert.CreatedAt);
        }

    }
}
