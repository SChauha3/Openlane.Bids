namespace Openlane.Bids.Shared.Infrastructure.Services.Queues
{
    public interface IQueueService<T>
    {
        Task<string> PublishAsync(T message);
        Task ConsumeAsync(Func<T, Task> consumerHandlerAsync);
    }
}
