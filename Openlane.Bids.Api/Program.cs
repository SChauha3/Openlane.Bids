using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog;
using Openlane.Bids.Shared.Extensions;
//using Openlane.Bids.Api.Validators;
//using FluentValidation;
//using FluentValidation.AspNetCore;
using System;
using Serilog.Sinks.File;
using Openlane.Bids.Shared.Models;
using Openlane.Bids.Api;
using Openlane.Bids;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

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

//builder.Services.AddScoped<IValidator<Bid>, BidValidator>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();

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
    return await controller.GetBids(auctionId, carId, cursor, pageSize);
});

appMapGroup.MapPost("api/bids", (BidController controller, [FromBody] Bid bid) => controller.PostBid(bid));


app.Run();


[JsonSerializable(typeof(Result<string>))]
internal partial class ResultJsonContext:JsonSerializerContext
{
}