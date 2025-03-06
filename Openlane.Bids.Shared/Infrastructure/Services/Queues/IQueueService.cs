namespace Openlane.Bids.Shared.Infrastructure.Services.Queues
{
    public interface IQueueService<T>
    {
        Task PublishAsync(T message);
        Task ConsumeAsync(Func<T, Task> consumerHandlerAsync);
    }
}
