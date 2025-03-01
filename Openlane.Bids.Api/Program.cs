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

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, BidJsonContext.Default);
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

builder.Services.AddTransient<BidController>();
//Sql Server
builder.Services.AddRepository();
// RabbitMQ
await builder.Services.AddQueueService();
//Redis Cache
builder.Services.AddCacheService();

var app = builder.Build();

var appMapGroup = app.MapGroup("/");
appMapGroup.MapGet("/", () => { return "I'm available"; });

appMapGroup.MapGet("api/bids", async (
    BidController controller,
    int auctionId,
    int carId,
    int cursor,
    int pageSize = 10) =>
{
    await controller.GetBids(auctionId, carId, cursor, pageSize);
});

appMapGroup.MapPost("api/bids", (BidController controller, Bid bid) => controller.PostBid(bid));


app.Run();