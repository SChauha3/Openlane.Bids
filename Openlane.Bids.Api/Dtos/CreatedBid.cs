namespace Openlane.Bids.Api.Dtos
{
    public record CreatedBid(
        int AuctionId,
        int CarId,
        string BidderName,
        decimal Amount
        );
}
