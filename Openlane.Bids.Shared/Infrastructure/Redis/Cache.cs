using Openlane.Bids.Shared.Dtos;
using StackExchange.Redis;
using System.Text.Json;

namespace Openlane.Bids.Shared.Infrastructure.Redis
{
    public class Cache : ICache
    {
        private readonly IConnectionMultiplexer _redis;
        public Cache(IConnectionMultiplexer redis) 
        {
            _redis = redis;
        }

        public async Task SetCache(string key, IEnumerable<Bid> bids)
        {
            var cache = _redis.GetDatabase();
            await cache.StringSetAsync(key, JsonSerializer.Serialize(bids, BidJsonContext.Default.Bid), TimeSpan.FromMinutes(5));
        }

        public async Task<Bid> GetCache(string key)
        {
            var cache = _redis.GetDatabase();
            var cachedData = await cache.StringGetAsync(key);
            return JsonSerializer.Deserialize<Bid>(cachedData, BidJsonContext.Default.Bid);
        }
    }
}
