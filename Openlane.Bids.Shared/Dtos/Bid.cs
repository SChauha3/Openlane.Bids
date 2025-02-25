namespace Openlane.Bids.Shared.Dtos
{
    public class Bid
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public Guid CarId { get; set; }
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;
    }
}
