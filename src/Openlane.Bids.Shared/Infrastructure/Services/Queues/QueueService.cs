using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Openlane.Bids.Shared.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Openlane.Bids.Shared.Infrastructure.Services.Queues
{
    public class QueueService : IQueueService<BidEvent>
    {
        private readonly IChannel _channel;
        private readonly ILogger<IQueueService<BidEvent>> _logger;
        public QueueService(ILogger<IQueueService<BidEvent>> logger, IChannel channel)
        {
            _logger = logger;
            _channel = channel;
        }

        public async Task PublishAsync(BidEvent bid)
        {
            string correlationId = string.Empty;
            try
            {
                var bidJson = JsonSerializer.Serialize(bid, BidJsonContext.Default.BidEvent);

                var body = Encoding.UTF8.GetBytes(bidJson);
                await _channel.BasicPublishAsync(
                    exchange: "openlane-bids",
                    routingKey: "openlane.bid.creation",
                    mandatory: true,
                    body: body);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "bid with {AuctionId} and {carId} could not be serialized", bid.AuctionId, bid.CarId);
                throw;
            }
            catch (PublishException ex)
            {
                _logger.LogError(ex, "bid with {AuctionId} and {carId} could not be published", bid.AuctionId, bid.CarId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred for your bid with {AuctionId} and {carId}", bid.AuctionId, bid.CarId);
                throw;
            }
        }

        public async Task ConsumeAsync(Func<BidEvent, Task> ConsumerHandlerAsync)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received event: {Message}", message);

                try
                {
                    var bid = JsonSerializer.Deserialize<BidEvent>(message, BidJsonContext.Default.BidEvent);

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