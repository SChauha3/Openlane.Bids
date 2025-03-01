using System.Text;
using System.Text.Json;
using System.Threading.Channels;
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

        public async Task PublishAsync(Bid bid)
        {
            try
            {
                var bidJson = JsonSerializer.Serialize(bid, BidJsonContext.Default.Bid);

                var body = Encoding.UTF8.GetBytes(bidJson);
                await _channel.BasicPublishAsync(
                    exchange: "openlane-bids",
                    routingKey: "openlane.bid.creation",
                    mandatory: true,
                    body: body);
            }
            catch(JsonException ex)
            {
                throw;
            }
            catch(PublishException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ConsumeAsync(Action<Bid> consumerHandler)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {Message}", message);

                try
                {
                    var bid = JsonSerializer.Deserialize<Bid>(message, BidJsonContext.Default.Bid);

                    consumerHandler.Invoke(bid);
                }
                catch(JsonException ex)
                {
                    throw;
                }
                catch(Exception ex)
                {
                    throw;
                }
                
                // Acknowledge the message
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(queue: "bids-queue", autoAck: false, consumer: consumer);
        }
    }
}
