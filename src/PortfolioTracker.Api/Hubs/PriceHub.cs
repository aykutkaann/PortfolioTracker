using Microsoft.AspNetCore.SignalR;
using PortfolioTracker.Application.Interfaces;
using System.Security.Claims;

namespace PortfolioTracker.Api.Hubs
{
    public class PriceHub : Hub
    {
        public async Task JoinSymbolGroup(string symbol)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, symbol.ToUpper());
        }

        public async Task LeaveSymbolGroup(string symbol)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol.ToUpper());
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnConnectedAsync();
        }

    }
}
