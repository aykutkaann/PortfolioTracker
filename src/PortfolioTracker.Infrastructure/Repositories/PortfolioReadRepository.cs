using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PortfolioTracker.Application.DTOs.Portfolio;
using PortfolioTracker.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PortfolioTracker.Infrastructure.Repositories
{
    public class PortfolioReadRepository(IConfiguration configuration, ICacheService cacheService, ICurrentUserService currentUserService) :IPortfolioReadService
    {


        private IDbConnection CreateConnection() => new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));


        public async Task<List<PortfolioDashboardDto>> GetDashboardAsync(CancellationToken ct)
        {

            var sql = @"SELECT p.""Id"" as PortfolioId, 
                               p.""Name"" as PortfolioName,
                               COUNT(h.""Id"") as TotalHoldings,
                               COALESCE(SUM(h.""Quantity"" * h.""AverageBuyPrice""), 0) as TotalInvested
                        FROM ""Portfolios"" p
                        LEFT JOIN ""Holdings"" h ON h.""PortfolioId"" = p.""Id""
                        WHERE p.""UserId"" = @UserId
                        GROUP BY p.""Id"", p.""Name""
                        ";

            var portfolios = @"SELECT h.""PortfolioId"", h.""Symbol"", h.""Quantity""
                                FROM ""Holdings"" h
                                INNER JOIN ""Portfolios"" p ON p.""Id"" = h.""PortfolioId""
                                WHERE p.""UserId"" = @UserId
                                ";



            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            using var connection = CreateConnection();
            var rows = await connection.QueryAsync<DashboardRawRow>(sql, new { UserId = userId });
            var holdings = await connection.QueryAsync<HoldingRaw>(portfolios, new { UserId = userId });

            var portfolioValues = new Dictionary<Guid, decimal>();

            foreach (var h in holdings)
            {
                var price = await cacheService.GetAsync<decimal?>($"price:{h.Symbol}") ?? 0;
                var value = h.Quantity * price;

                if (portfolioValues.ContainsKey(h.PortfolioId))
                    portfolioValues[h.PortfolioId] += value;
                else
                    portfolioValues[h.PortfolioId] = value;
            }


            var result = new List<PortfolioDashboardDto>();

            foreach (var row in rows)
            {
                var currentValue = portfolioValues.GetValueOrDefault(row.PortfolioId, 0);
                var profitLoss = currentValue - row.TotalInvested;
                var profitLossPercent = row.TotalInvested == 0 ? 0 : (profitLoss / row.TotalInvested) * 100;

                result.Add(new PortfolioDashboardDto(
                    row.PortfolioId, row.PortfolioName, row.TotalHoldings,
                    row.TotalInvested, currentValue, profitLoss,
                    Math.Round(profitLossPercent, 2)));
            }




            return result;
        }

        public async Task<List<HoldingDetailDto>> GetHoldingDetailsAsync(Guid portfolioId, CancellationToken ct)
        {
            var sql = @"
        SELECT h.""Id"" as HoldingId,
               h.""Symbol"",
               h.""AssetType"",
               h.""Quantity"",
               h.""AverageBuyPrice"",
               (h.""Quantity"" * h.""AverageBuyPrice"") as TotalInvested
        FROM ""Holdings"" h
        WHERE h.""PortfolioId"" = @PortfolioId";

            using var connection = CreateConnection();
            var rows = (await connection.QueryAsync <HoldingRaw> (sql, new { PortfolioId = portfolioId })).ToList();

            var result = new List<HoldingDetailDto>();

            foreach (var row in rows)
            {
                var currentPrice = await cacheService.GetAsync<decimal?>($"price:{row.Symbol}") ?? 0;
                var currentValue = row.Quantity * currentPrice;
                var profitLoss = currentValue - row.TotalInvested;
                var profitLossPercent = row.TotalInvested == 0 ? 0 : (profitLoss / row.TotalInvested) * 100;

                result.Add(new HoldingDetailDto(
                    row.HoldingId, row.Symbol, row.AssetType, row.Quantity,
                    row.AverageBuyPrice, currentPrice, row.TotalInvested,
                    currentValue, profitLoss, Math.Round(profitLossPercent, 2)));
            }

            return result;
        }


        private class DashboardRawRow
        {
            public Guid PortfolioId { get; set; }
            public string PortfolioName { get; set; }
            public int TotalHoldings { get; set; }
            public decimal TotalInvested { get; set; }
        }


        private class HoldingRaw
        {
            public Guid PortfolioId { get; set; }
            public Guid HoldingId { get; set; }
            public string Symbol { get; set; }
            public decimal TotalInvested { get; set; }
            public string AssetType { get; set; }
            public decimal AverageBuyPrice { get; set; }

            public decimal Quantity { get; set; }
        }

    }
}
