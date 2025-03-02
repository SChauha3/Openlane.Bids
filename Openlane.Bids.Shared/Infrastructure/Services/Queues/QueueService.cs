using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Openlane.Bids.Shared.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Openlane.Bids.Shared.Infrastructure.Services.Queues
{
    public class QueueService : IQueueService<Bid>
    {
        private readonly IChannel _channel;
        private readonly ILogger<IQueueService<Bid>> _logger;
        public QueueService(ILogger<IQueueService<Bid>> logger, IChannel channel)
        {
            _logger = logger;
            _channel = channel;
        }

        public async Task<string> PublishAsync(Bid bid)
        {
            string correlationId = string.Empty;
            try
            {
                correlationId = Guid.NewGuid().ToString();
                var props = new BasicProperties
                {
                    CorrelationId = correlationId,
                };

                var bidJson = JsonSerializer.Serialize(bid, BidJsonContext.Default.Bid);

                var body = Encoding.UTF8.GetBytes(bidJson);
                await _channel.BasicPublishAsync(
                    exchange: "openlane-bids",
                    routingKey: "openlane.bid.creation",
                    mandatory: true,
                    basicProperties: props,
                    body: body);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "bid with {AuctionId} and {carId} could not be serialized", bid.AuctionId, bid.CarId);
                correlationId = string.Empty;
            }
            catch (PublishException ex)
            {
                _logger.LogError(ex, "bid with {AuctionId} and {carId} could not be published", bid.AuctionId, bid.CarId);
                correlationId = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred for your bid with {AuctionId} and {carId}", bid.AuctionId, bid.CarId);
                correlationId = string.Empty;
            }

            return correlationId;
        }

        public async Task ConsumeAsync(Func<Bid, Task> ConsumerHandlerAsync)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message for {crrelationId}: {Message}", ea.BasicProperties.CorrelationId, message);

                try
                {
                    var bid = JsonSerializer.Deserialize<Bid>(message, BidJsonContext.Default.Bid);

                    await ConsumerHandlerAsync.Invoke(bid);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "bid with could not be serialized");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred for {correlationId}", ea.BasicProperties.CorrelationId);
                    throw;
                }

                // Acknowledge the message
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(queue: "bids-queue", autoAck: false, consumer: consumer);
        }
    }
}