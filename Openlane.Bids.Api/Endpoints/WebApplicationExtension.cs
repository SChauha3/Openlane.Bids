using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
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
            })
                .WithName("GetBids")
                .WithMetadata(new OpenApiOperation
                {
                    Summary = "Get bids by AuctionId and CarId",
                    Description = "Get bids by AuctionId and CarId",
                    Tags = new List<OpenApiTag> { new() { Name = "Bids" } },
                    Parameters = {
                    new OpenApiParameter
                    {
                        Name = "auctionId",
                        In = ParameterLocation.Query, 
                        Required = true,
                        Schema = new OpenApiSchema { Type = "integer" }
                    },
                    new OpenApiParameter
                    {
                        Name = "carId",
                        In = ParameterLocation.Query, 
                        Required = true,
                        Schema = new OpenApiSchema { Type = "integer" }
                    },
                    new OpenApiParameter
                    {
                        Name = "cursor",
                        In = ParameterLocation.Query, 
                        Required = true,
                        Schema = new OpenApiSchema { Type = "integer" }
                    },
                    new OpenApiParameter
                    {
                        Name = "pageSize",
                        In = ParameterLocation.Query, 
                        Required = true,
                        Schema = new OpenApiSchema { Type = "integer" }
                    }}
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
            })
                .WithName("PostBids")
                .WithMetadata(new OpenApiOperation
                {
                    Summary = "Post Bids",
                    Description = "Post Bids",
                    Tags = new List<OpenApiTag> { new() { Name = "Bids" } },
                    RequestBody = new OpenApiRequestBody
                    {
                        Description = "Bid request body",
                        Required = true,
                        Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                { 
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["AuctionId"] = new OpenApiSchema { Type = "string" },
                                        ["CarId"] = new OpenApiSchema { Type = "string" },
                                        ["BidderName"] = new OpenApiSchema { Type = "string" },
                                        ["Amount"] = new OpenApiSchema { Type = "number", Format = "double" }
                                    },
                                    Required = new HashSet<string> {"AuctionId", "CarId", "BidderName", "Amount" }
                                }
                            }
                        }
                    }
                });

            return app;
        }
    }
}