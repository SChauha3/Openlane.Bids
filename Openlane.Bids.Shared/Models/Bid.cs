using System.ComponentModel.DataAnnotations;

namespace Openlane.Bids.Shared.Models
{
    public class Bid
    {
        public int? Id { get; set; } = null;
        public int AuctionId { get; set; }
        public int CarId { get; set; }
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;
    }
}
