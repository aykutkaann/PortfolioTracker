using PortfolioTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.DTOs.Portfolio
{
    public record CreatePortfolioRequest(string Name);

    public record PortfolioResponse(Guid Id, string Name, DateTime CreatedAt, List<HoldingResponse> Holdings);

    public record HoldingResponse(Guid Id, string Symbol, string AssetType, decimal Quantity, decimal AvarageBuyPrice);

    public record AddTransactionRequest(string Symbol, string AssetType, string Type, decimal Quantity, decimal PricePerUnit);

    public record TransactionResponse(Guid Id, string Symbol, string Type, decimal Quantity, decimal PricePerUnit, DateTime ExecutedAt);



}
