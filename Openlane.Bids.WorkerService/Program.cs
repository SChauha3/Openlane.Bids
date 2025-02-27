using Openlane.Bids.WorkerService;
using Openlane.Bids.Shared.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

//Sql Server
builder.Services.AddRepository();
// RabbitMQ
await builder.Services.AddQueueService();
//Redis Cache
builder.Services.AddCacheService();

var host = builder.Build();
host.Run();
