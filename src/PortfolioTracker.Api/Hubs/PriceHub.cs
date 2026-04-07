using Microsoft.AspNetCore.SignalR;
using PortfolioTracker.Application.Interfaces;

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
    }
}
