using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.Services.Caches;
using Openlane.Bids.Shared.Infrastructure.Services.Queues;
using Openlane.Bids.Shared.Models;

namespace Openlane.Bids.Api
{
    public class BidController
    {
        private readonly ILogger<BidController> _logger;
        private readonly IQueueService<Shared.Models.Bid> _queue;
        private readonly ICacheService _cache;
        private readonly IRepository _repository;

        public BidController(
            ILogger<BidController> logger, 
            IQueueService<Shared.Models.Bid> queue,
            ICacheService cache,
            IRepository repository)
        {
            _logger = logger;
            _queue = queue;
            _cache = cache;
            _repository = repository;
        }

        public async Task<Result<string>> PostBid(Bid bid)
        {
            var correlationId = await _queue.PublishAsync(bid);
            _logger.LogInformation("bid enqueued with {correlationId}", correlationId);
            
            if (string.IsNullOrEmpty(correlationId)) 
            {
                return Result<string>.Failure("Could not be stored. Please try again later");
            }

            return Result<string>.Success(correlationId);
        }

        public async Task<IEnumerable<Bid>> GetBids(
            int auctionId,
            int carId,
            int cursor, 
            int pageSize)
        {
            _logger.LogInformation("request received for {auctionId}, {carId}, {cursor}", auctionId, carId, cursor);
            var cacheKey = $"bids:{auctionId}:{carId}:{cursor}:{pageSize}";
            var cachedData = await _cache.GetCache(cacheKey);

            if (cachedData.Any())
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);

                return cachedData;
            }

            var bids = await _repository.GetAsync(auctionId, carId, cursor, pageSize);

            int? nextCursor = bids.Count() == pageSize ? bids.Last().Id : null;

            await _cache.SetCache(cacheKey, bids);
            _logger.LogInformation("Cache set for {CacheKey}", cacheKey);

            return bids;
        }
    }
}