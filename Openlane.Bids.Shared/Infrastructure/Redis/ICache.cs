using Openlane.Bids.Shared.Dtos;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlane.Bids.Shared.Infrastructure.Redis
{
    public interface ICache
    {
        
        Task SetCache(string key, IEnumerable<Bid> bids);
        Task<Bid> GetCache(string key);

    }
}
