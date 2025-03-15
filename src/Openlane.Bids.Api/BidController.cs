using FluentValidation;
using Openlane.Bids.Api.Dtos;
using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.Services.Caches;
using Openlane.Bids.Shared.Infrastructure.Services.Queues;
using Openlane.Bids.Shared.Models;

namespace Openlane.Bids.Api
{
    public class BidController
    {
        private readonly ILogger<BidController> _logger;
        private readonly IQueueService<Shared.Models.BidEvent> _queue;
        private readonly ICacheService _cache;
        private readonly IRepository _repository;
        private readonly IValidator<CreateBid> _validator;

        public BidController(
            ILogger<BidController> logger, 
            IQueueService<Shared.Models.BidEvent> queue,
            ICacheService cache,
            IRepository repository,
            IValidator<CreateBid> validator)
        {
            _logger = logger;
            _queue = queue;
            _cache = cache;
            _repository = repository;
            _validator = validator;
        }

        public async Task<Result<string>> PostBidAsync(CreateBid bid)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(bid);

                if (!validationResult.IsValid)
                {
                    return Result<string>.Failure(ErrorType.ValidationError, "Invalid request");
                }

                var bidEvent = new BidEvent
                {
                    Amount = bid.Amount,
                    AuctionId = bid.AuctionId,
                    BidderName = bid.BidderName,
                    CarId = bid.CarId,
                    TransactionId = Guid.NewGuid(),
                };

                await _queue.PublishAsync(bidEvent);
                _logger.LogInformation("bid enqueued with {transactionId}", bidEvent.TransactionId);

                return Result<string>.Success(bidEvent.TransactionId.ToString());
            }
            catch (Exception)
            {
                return Result<string>.Failure(ErrorType.InternalServerError, "Could not be stored. Please try again later");
            }
        }

        public async Task<Result<IEnumerable<CreatedBid>>> GetBids(
            int auctionId,
            int carId,
            int cursor,
            int pageSize)
        {
            _logger.LogInformation("request received for {auctionId}, {carId}, {cursor}", auctionId, carId, cursor);
            var cacheKey = $"bids:{auctionId}:{carId}:{cursor}:{pageSize}";
            var cachedData = await _cache.GetCache(cacheKey);

            var createdBids = new List<CreatedBid>();
            if (cachedData.Any())
            {
                _logger.LogInformation("Bids are fetched from cache {CacheKey}", cacheKey);

                var bidDtos = MapModelToDto(cachedData);
                return Result<IEnumerable<CreatedBid>>.Success(bidDtos);
            }

            var bids = await _repository.GetAsync(auctionId, carId, cursor, pageSize);

            if (!bids.Any())
            {
                return Result<IEnumerable<CreatedBid>>.Failure(ErrorType.NotFoundError, "Not found");
            }

            await _cache.SetCache(cacheKey, bids);
            _logger.LogInformation("Cache set for {CacheKey}", cacheKey);

            return Result<IEnumerable<CreatedBid>>.Success(MapModelToDto(bids));
        }

        private IEnumerable<CreatedBid> MapModelToDto(IEnumerable<Bid> bids)
        {
            var createdBids = new List<CreatedBid>();
            foreach (var bid in bids)
            {
                createdBids.Add(
                    new CreatedBid(
                        Id: bid.Id,
                        TransactionId: bid.TransactionId,
                        AuctionId: bid.AuctionId,
                        CarId: bid.CarId,
                        BidderName: bid.BidderName,
                        Amount: bid.Amount));
            }

            return createdBids;
        }
    }
}