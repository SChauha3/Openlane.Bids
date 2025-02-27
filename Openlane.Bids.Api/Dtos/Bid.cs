namespace Openlane.Bids.Shared.Dtos
{
    public class Bid
    {
        public int AuctionId { get; set; }
        public int CarId { get; set; }
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;
    }
}
