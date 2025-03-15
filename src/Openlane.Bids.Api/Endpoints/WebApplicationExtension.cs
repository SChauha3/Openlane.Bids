using Microsoft.AspNetCore.Mvc;
using Openlane.Bids.Api.Dtos;

namespace Openlane.Bids.Api.Endpoints
{
    public static class WebApplicationExtension
    {
        public static WebApplication MapRoutes(this WebApplication app)
        {
            var appMapGroup = app.MapGroup("/");

            appMapGroup.MapGet("api/bids", async (
                [FromServices] BidController controller,
                [FromQuery] int auctionId,
                [FromQuery] int carId,
                [FromQuery] int cursor,
                [FromQuery] int pageSize) =>
            {
                var response = await controller.GetBids(auctionId, carId, cursor, pageSize);
                if (response.IsSuccess)
                    return Results.Ok(response.Value);

                return response.ErrorType switch
                {
                    ErrorType.NotFoundError => Results.Problem(response.ErrorMessage, statusCode: StatusCodes.Status404NotFound),
                    _ => Results.Problem(response.ErrorMessage, statusCode: StatusCodes.Status500InternalServerError)
                };
            });

            appMapGroup.MapPost("api/bids", async (BidController controller, [FromBody] CreateBid bid) =>
            {
                var response = await controller.PostBidAsync(bid);
                if (response.IsSuccess)
                    return Results.Ok(response);

                return response.ErrorType switch
                {
                    ErrorType.ValidationError => Results.Problem(response.ErrorMessage, statusCode: StatusCodes.Status400BadRequest),
                    _ => Results.Problem(response.ErrorMessage, statusCode: StatusCodes.Status500InternalServerError)
                };
            });
            
            return app;
        }
    }
}