using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PortfolioTracker.Api.Hubs;
using PortfolioTracker.Domain.Enums;
using PortfolioTracker.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Api.Messaging.Consumers

{
    public class PriceUpdatedConsumer :IConsumer<PriceUpdatedEvent>
    {
        private readonly IHubContext<PriceHub> _hubContext;
        private readonly ILogger<PriceUpdatedConsumer> _logger;

        public PriceUpdatedConsumer(IHubContext<PriceHub> hubContext, ILogger<PriceUpdatedConsumer> logger)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<PriceUpdatedEvent> context)

        {
            try
            {
                var symbol = context.Message.Symbol.ToUpper();

                var updateMessage = new
                {
                    Symbol = symbol,
                    Price = context.Message.Price,
                    TimeStamp = context.Message.Timestamp
                };

                await _hubContext.Clients.Groups(symbol).SendAsync("PriceUpdate", updateMessage);

                await _hubContext.Clients.All
                         .SendAsync("GlobalPriceUpdate", updateMessage);

                _logger.LogDebug("Real-time update sent for {Symbol}: {Price}", symbol, context.Message.Price);


            }
            catch(Exception err)
            {
                _logger.LogError(err, "SignalR broadcast failed for {Symbol}", context.Message.Symbol);
            }
        }
        
    }
}
