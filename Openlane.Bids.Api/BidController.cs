using Microsoft.AspNetCore.Mvc;
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

        public async Task PostBid(Bid bid)
        {
            //if(!ModelState.IsValid) 
            //{
            //    return BadRequest(ModelState);
            //}

            var bidModel = new Shared.Models.Bid()
            {
                Amount = bid.Amount,
                AuctionId = bid.AuctionId,
                BidderName = bid.BidderName,
                CarId = bid.CarId,
                Timestamp = bid.Timestamp,
            };

            await _queue.PublishAsync(bidModel);
            _logger.LogInformation("Bid enqueued in RabbitMQ");
        }

        public async Task<IEnumerable<Bid>> GetBids(
            int auctionId,
            int carId,
            int cursor, 
            int pageSize = 10)
        {
            var cacheKey = $"bids:{auctionId}:{carId}:{cursor}:{pageSize}";
            var cachedData = await _cache.GetCache(cacheKey);

            if (cachedData.Any())
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return cachedData;
            }

            var bids = await _repository.GetAsync(auctionId, pageSize, cursor);

            int? nextCursor = bids.Count() == pageSize ? bids.Last().Id : null;

            var response = new
            {
                Bids = bids,
                NextCursor = nextCursor
            };

            await _cache.SetCache(cacheKey, bids);
            _logger.LogInformation("Cache set for {CacheKey}", cacheKey);

            return new List<Bid>();
        }
    }
}