using Openlane.Bids.WorkerService;
using Openlane.Bids.Shared.Extensions;
using Openlane.Bids.Shared.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

var serilogConfig = new LoggerConfiguration().ReadFrom.Configuration(config.GetSection("Serilog")).CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(serilogConfig);

//ServiceCollectionExtension.Initialize(
//    config.GetSection("AppSettings:InfraSettings")?.Get<InfraSettings>() ?? throw new ApplicationException("configuration is not valid"));
//Sql Server
builder.Services.AddRepository();
// RabbitMQ
await builder.Services.AddQueueService();
//Redis Cache
builder.Services.AddCacheService();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();