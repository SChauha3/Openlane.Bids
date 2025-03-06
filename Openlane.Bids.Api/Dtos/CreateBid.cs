namespace Openlane.Bids.Api.Dtos
{
    public record CreateBid(
        int AuctionId,
        int CarId,
        string BidderName,
        decimal Amount
        );
}
