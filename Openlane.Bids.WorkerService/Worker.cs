using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.Services.Queues;
using Openlane.Bids.Shared.Infrastructure.Services.Caches;
using Openlane.Bids.Shared.Models;

namespace Openlane.Bids.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueService<BidEvent> _queueService;
        private readonly ICacheService _cacheService;
        private readonly IRepository _repository;

        public Worker(
            ILogger<Worker> logger, 
            IQueueService<BidEvent> queueService, 
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

        public async Task ConsumerHandlerAsync(BidEvent bid)
        {
            var bidModel = new Bid()
            {
                Amount = bid.Amount,
                AuctionId = bid.AuctionId,
                BidderName = bid.BidderName,
                CarId = bid.CarId,
                Timestamp = DateTimeOffset.UtcNow,
                TransactionId = bid.TransactionId,
            };
            await _repository.SaveAsync(bidModel);
        }
    }
}
