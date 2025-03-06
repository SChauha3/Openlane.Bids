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
using Openlane.Bids.Api.Endpoints;

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

builder.Services.AddScoped<IValidator<CreateBid>, CreateBidValidator>();

builder.Services.AddScoped<BidController>();
//Sql Server
builder.Services.AddRepository();
// RabbitMQ
await builder.Services.AddQueueService();
//Redis Cache
builder.Services.AddCacheService();

var app = builder.Build();

//Map Routes
app.MapRoutes();

app.Run();


[JsonSerializable(typeof(Result<string>))]
[JsonSerializable(typeof(CreateBid))]
[JsonSerializable(typeof(CreatedBid))]
[JsonSerializable(typeof(IEnumerable<CreatedBid>))]
[JsonSerializable(typeof(ProblemDetails))]
internal partial class ResultJsonContext:JsonSerializerContext
{
}