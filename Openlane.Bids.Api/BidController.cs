using Microsoft.AspNetCore.Mvc;
using Openlane.Bids.Shared;
using Openlane.Bids.Shared.Dtos;
using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.RabbitMq;
using Openlane.Bids.Shared.Infrastructure.Redis;
using System.Text.Json;

namespace Openlane.Bids.Api
{
    [ApiController]
    [Route("api/bids")]
    public class BidController : ControllerBase
    {
        private readonly ILogger<BidController> _logger;
        private readonly IQueue _queue;
        private readonly ICache _cache;
        private readonly IRepository _repository;

        public BidController(
            ILogger<BidController> logger, 
            IQueue queue,
            ICache cache,
            IRepository repository)
        {
            _logger = logger;
            _queue = queue;
            _cache = cache;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> PostBid([FromBody] Bid bid)
        {
            bid.Id = Guid.NewGuid();
            var bidJson = JsonSerializer.Serialize<Bid>(bid, BidJsonContext.Default.Bid);

            await _queue.PublishAsync(bidJson);
            _logger.LogInformation("Bid enqueued in RabbitMQ: {BidId}", bid.Id);

            return Accepted();
        }

        [HttpGet]
        public async Task<IActionResult> GetBids(
            [FromQuery] Guid auctionId, 
            [FromQuery] Guid cursor, 
            [FromQuery] int pageSize = 10)
        {
            var cacheKey = $"bids:{auctionId}:{cursor}:{pageSize}";
            var cachedData = await _cache.GetCache(cacheKey);

            if (cachedData != null)
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return Ok(cachedData);
            }

            var bids = await _repository.GetAsync(auctionId, pageSize, cursor);

            Guid? nextCursor = bids.Count() == pageSize ? bids.Last().Id : null;

            var response = new
            {
                Bids = bids,
                NextCursor = nextCursor
            };

            await _cache.SetCache(cacheKey, bids);
            _logger.LogInformation("Cache set for {CacheKey}", cacheKey);

            return Ok(response);
        }
    }
}