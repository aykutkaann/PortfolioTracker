using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Application.DTOs.Portfolio;
using System;
using System.Collections.Generic;
using System.Text;
using PortfolioTracker.Domain.Entities;
using Microsoft.AspNetCore.Http;
using PortfolioTracker.Domain.Enums;

namespace PortfolioTracker.Application.Services
{
    public class PortfolioService(IPortfolioRepository portfolioRepository, ICurrentUserService currentUserService) : IPortfolioService
    {

        public async Task<PortfolioResponse> CreateAsync(CreatePortfolioRequest request, CancellationToken ct)
        {

            var userId =  currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var portfolio = new Portfolio(userId, request.Name);

            await portfolioRepository.AddAsync(portfolio, ct);
            return MapToResponse(portfolio);



        }

        public async Task<List<PortfolioResponse>> GetAllAsync( CancellationToken ct)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var portfolios = await portfolioRepository.GetByUserIdAsync(userId, ct);

            return portfolios.Select(MapToResponse).ToList();

        }

        public async Task<PortfolioResponse> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var portfolio = await portfolioRepository.GetByIdAsync(id, ct);

            if (portfolio == null) throw new Exception("Portfolio not found.");
            if (portfolio.UserId != currentUserService.UserId) throw new UnauthorizedAccessException("You dont have access for this operation.");

            return MapToResponse(portfolio);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var portfolio = await portfolioRepository.GetByIdAsync(id, ct);

            if (portfolio == null) throw new Exception("Portfolio not found.");
            if (portfolio.UserId != currentUserService.UserId) throw new UnauthorizedAccessException("You dont have access for this operation.");

             await portfolioRepository.DeleteAsync(portfolio, ct);
             

        }

        public async Task<TransactionResponse> AddTransactionAsync(Guid portfolioId, AddTransactionRequest request, CancellationToken ct)
        {
            var portfolio = await portfolioRepository.GetByIdAsync(portfolioId, ct);

            if (portfolio == null) throw new Exception("Portfolio not found.");
            if (portfolio.UserId != currentUserService.UserId) throw new UnauthorizedAccessException("You dont have access for this operation.");

            var assetType = Enum.Parse<AssetType>(request.AssetType);

            var transactionType = Enum.Parse<TransactionType>(request.Type);

            var holding = portfolio.Holdings.FirstOrDefault(h => h.Symbol == request.Symbol);

            if(transactionType == TransactionType.Buy)
            {
                if(holding != null)
                {
                    holding.AddQuantity(request.Quantity, request.PricePerUnit);
                }
                else
                {
                    holding = new Holding(portfolio.Id, request.Symbol, assetType, request.Quantity, request.PricePerUnit);

                    portfolio.Holdings.Add(holding);
                }
            }
            else
            {
                if (holding == null)
                {
                    throw new Exception("Cant sell what you dont have.");
                }

                holding.RemoveQuantity(request.Quantity);


            }
            var transaction = new Transaction(holding.Id, transactionType, request.Quantity, request.PricePerUnit);
            holding.Transactions.Add(transaction);

            await portfolioRepository.UpdateAsync(portfolio, ct);

            return new TransactionResponse(transaction.Id, holding.Symbol, transactionType.ToString(), transaction.Quantity, transaction.PricePerUnit, transaction.ExecutedAt);

        }

        private static PortfolioResponse MapToResponse(Portfolio portfolio)
        {
            var holdings = portfolio.Holdings.Select(h => new HoldingResponse(
                h.Id,
                h.Symbol,
                h.AssetType.ToString(),
                h.Quantity,
                h.AverageBuyPrice
            )).ToList();

            return new PortfolioResponse(portfolio.Id, portfolio.Name, portfolio.CreatedAt, holdings);
        }
    }
}
