using Openlane.Bids.Shared;
using Openlane.Bids.Shared.Dtos;
using Openlane.Bids.Shared.Infrastructure.Database;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Openlane.Bids.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IChannel _channel;
        private readonly IRepository _repository;

        public Worker(ILogger<Worker> logger, IChannel channel)
        {
            _logger = logger;
            _channel = channel;
            _channel.QueueDeclareAsync(queue: "bids-queue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var bid = JsonSerializer.Deserialize<Bid>(message, BidJsonContext.Default.Bid);

                if (bid != null)
                {
                    await _repository.SaveAsync(bid);
                }
            };

            _channel.BasicConsumeAsync(queue: "bids", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
