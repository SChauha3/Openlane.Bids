using System.Text;
using RabbitMQ.Client;

namespace Openlane.Bids.Shared.Infrastructure.RabbitMq
{
    public class Queue: IQueue
    {
        private readonly IChannel _channel;
        public Queue(IChannel channel)
        {
            _channel = channel;
        }

        public async Task PublishAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "",
                mandatory: true,
                body: body);
        }
    }
}
