using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog;
using Openlane.Bids.Shared.Extensions;
using Openlane.Bids.Api;
using Openlane.Bids;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Openlane.Bids.Api.Validators;
using Openlane.Bids.Api.Dtos;
using Azure;
using Openlane.Bids.Shared.Models;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, BidJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(1, ResultJsonContext.Default);
});

// Serilog configuration for structured logging
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();

// builder.Services.AddValidatorsFromAssemblyContaining<CreateBidValidator>();
builder.Services.AddScoped<IValidator<CreateBid>, CreateBidValidator>();

builder.Services.AddScoped<BidController>();
//Sql Server
builder.Services.AddRepository();
// RabbitMQ
await builder.Services.AddQueueService();
//Redis Cache
builder.Services.AddCacheService();

var app = builder.Build();

var appMapGroup = app.MapGroup("/");
appMapGroup.MapGet("/", () => { return "Hey, I'm available"; });

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


app.Run();


[JsonSerializable(typeof(Result<string>))]
[JsonSerializable(typeof(CreateBid))]
[JsonSerializable(typeof(CreatedBid))]
[JsonSerializable(typeof(ProblemDetails))]
internal partial class ResultJsonContext:JsonSerializerContext
{
}