using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog;
using Openlane.Bids.Shared.Extensions;
using Openlane.Bids.Api.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using Openlane.Bids.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, BidDtoJsonContext.Default);
});

// Serilog configuration for structured logging
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.ConfigureLogging(logging =>
{
    logging.AddConsole();
});

builder.Services.AddValidatorsFromAssemblyContaining<BidValidator>();

builder.Services
    .AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BidValidator>());

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<BidValidator>();

// Add controllers and enable FluentValidation
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();

//Sql Server
builder.Services.AddRepository();
// RabbitMQ
await builder.Services.AddQueueService();
//Redis Cache
builder.Services.AddCacheService();

var app = builder.Build();

app.Run();