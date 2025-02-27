using FluentValidation;
using Openlane.Bids.Shared.Dtos;

namespace Openlane.Bids.Api.Validators
{
    public class BidValidator : AbstractValidator<Bid>
    {
        public BidValidator()
        {
            RuleFor(b => b.AuctionId)
                .NotEmpty().WithMessage("AuctionId is required.");

            RuleFor(b => b.CarId)
                .NotEmpty().WithMessage("CarId is required.");

            RuleFor(b => b.BidderName)
                .NotEmpty().WithMessage("BidderName is required.")
                .MaximumLength(100).WithMessage("BidderName must not exceed 100 characters.");

            RuleFor(b => b.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(b => b.Timestamp)
                .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Timestamp cannot be in the future.");
        }
    }

}
