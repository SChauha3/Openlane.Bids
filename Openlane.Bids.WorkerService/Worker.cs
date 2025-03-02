using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.Services.Queues;
using Openlane.Bids.Shared.Infrastructure.Services.Caches;
using Openlane.Bids.Shared.Models;

namespace Openlane.Bids.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueService<Bid> _queueService;
        private readonly ICacheService _cacheService;
        private readonly IRepository _repository;

        public Worker(
            ILogger<Worker> logger, 
            IQueueService<Bid> queueService, 
            ICacheService cacheService,
            IRepository repository)
        {
            _logger = logger;
            _queueService = queueService;
            _cacheService = cacheService;
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("starting worker service");

            while (!stoppingToken.IsCancellationRequested)
            {
                await _queueService.ConsumeAsync(ConsumerHandlerAsync);
            }
        }

        public async Task ConsumerHandlerAsync(Bid bid)
        {
            await _repository.SaveAsync(bid);
        }
    }
}
