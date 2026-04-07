using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Alert
{
    public record CreateAlertRequest(string Symbol, string AssetType, decimal TargetPrice, bool IsAbove);
}
