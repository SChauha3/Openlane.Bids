using Openlane.Bids.Shared.Dtos;

namespace Openlane.Bids.Shared.Infrastructure.Database
{
    public interface IRepository
    {
        Task SaveAsync(Bid bid);
        Task<IEnumerable<Bid>> GetAsync(Guid auctionId, int pageSize, Guid cursor);
    }
}
