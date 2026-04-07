using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PortfolioTracker.Api.Hubs;
using PortfolioTracker.Domain.Events;

namespace PortfolioTracker.Api.Messaging.Consumers
{
    public class AlertTriggeredConsumer:IConsumer<AlertTriggeredEvent>
    {
        private readonly ILogger<AlertTriggeredConsumer> _logger;
        private readonly IHubContext<PriceHub> _hubContext;

        public AlertTriggeredConsumer(ILogger<AlertTriggeredConsumer> logger, IHubContext<PriceHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<AlertTriggeredEvent> context)
        {
            var message = context.Message;
            var userGroup = $"user:{message.UserId}";

            try
            {
                var alertData = new
                {
                    message.AlertId,
                    message.UserId,
                    message.Symbol,
                    message.CurrentPrice,
                    message.TargetPrice,
                    TriggeredAt = DateTime.UtcNow

                };

                await _hubContext.Clients.Groups(userGroup)
                    .SendAsync("AlertTriggered", alertData);

                _logger.LogInformation("Price alert sended to user: {UserId} - {Symbool}", message.UserId, message.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending alert notification: {AlertId}", message.AlertId);
            }
        }
    }
}
