using System.ComponentModel.DataAnnotations;

namespace Openlane.Bids.Shared.Models
{
    public class Bid
    {
        [Key]
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public int CarId { get; set; }
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;
    }
}
