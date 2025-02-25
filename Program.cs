using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog;
using RabbitMQ.Client;
using Openlane.Bids.Shared;
using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.RabbitMq;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, BidJsonContext.Default);
});

// Serilog configuration
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IQueue, Queue>();
builder.Services.AddSingleton<IRepository, Repository>();

// RabbitMQ
var factory = new ConnectionFactory() { HostName = "localhost:6379" };
var connection = await factory.CreateConnectionAsync();
var channel = connection.CreateChannelAsync();

var app = builder.Build();

app.Run();