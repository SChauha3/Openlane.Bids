namespace Openlane.Bids.Shared.Models
{
    public class BidEvent
    {
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public int AuctionId { get; set; }
        public int CarId { get; set; }
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
