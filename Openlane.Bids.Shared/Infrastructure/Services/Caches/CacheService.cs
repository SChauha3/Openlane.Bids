using Microsoft.Extensions.Logging;
using Openlane.Bids.Shared.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Openlane.Bids.Shared.Infrastructure.Services.Caches
{
    public class CacheService : ICacheService
    {
        private readonly ILogger<ICacheService> _logger;
        private readonly IDatabase _database;
        public CacheService(ILogger<ICacheService> logger, IDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        public async Task SetCache(string key, IEnumerable<Bid> bids)
        {
            await _database.StringSetAsync(key, JsonSerializer.Serialize(bids, BidJsonContext.Default.Bid), TimeSpan.FromMinutes(5));
        }

        public async Task<Bid> GetCache(string key)
        {
            var cachedData = await _database.StringGetAsync(key);
            return JsonSerializer.Deserialize<Bid>(cachedData, BidJsonContext.Default.Bid);
        }
    }
}