using FluentValidation;
using Openlane.Bids.Api.Dtos;

namespace Openlane.Bids.Api.Validators
{
    public class CreateBidValidator : AbstractValidator<CreateBid>
    {
        public CreateBidValidator()
        {
            RuleFor(bid => bid.AuctionId)
            .GreaterThan(0)
            .WithMessage("AuctionId must be greater than 0");

        RuleFor(bid => bid.CarId)
            .GreaterThan(0)
            .WithMessage("CarId must be greater than 0");

        RuleFor(bid => bid.BidderName)
            .NotEmpty()
            .WithMessage("Bidder name is required")
            .MaximumLength(50)
            .WithMessage("Bidder name must be at most 50 characters");

        RuleFor(bid => bid.Amount)
            .GreaterThan(0)
            .WithMessage("Bid amount must be greater than 0");
        }
    }

}
