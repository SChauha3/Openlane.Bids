namespace Openlane.Bids.Api.Dtos
{
    public record CreatedBid(
        int Id,
        Guid TransactionId,
        int AuctionId,
        int CarId,
        string BidderName,
        decimal Amount
        );
}
