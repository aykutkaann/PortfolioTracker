using PortfolioTracker.Application.DTOs.Alert;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Interfaces
{
    public interface IAlertService
    {
        Task<AlertResponse> CreateAsync(CreateAlertRequest request, CancellationToken ct = default);
        Task<List<AlertResponse>> GetMyAlertsAsync(CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}

