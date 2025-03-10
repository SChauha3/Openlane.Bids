﻿using Openlane.Bids.Shared.Models;

namespace Openlane.Bids.Shared.Infrastructure.Services.Caches
{
    public interface ICacheService
    {
        Task SetCache(string key, IEnumerable<Bid> bids);
        Task<IEnumerable<Bid>> GetCache(string key);
    }
}
