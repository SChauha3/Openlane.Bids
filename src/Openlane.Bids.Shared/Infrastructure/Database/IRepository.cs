using Openlane.Bids.Shared.Models;

namespace Openlane.Bids.Shared.Infrastructure.Database
{
    public interface IRepository
    {
        Task SaveAsync(Bid bid);
        Task<IEnumerable<Bid>> GetAsync(int auctionId, int carId, int cursor, int pageSize);
    }
}
