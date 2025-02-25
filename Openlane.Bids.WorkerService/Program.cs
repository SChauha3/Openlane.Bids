using Openlane.Bids.Shared;
using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.RabbitMq;
using Openlane.Bids.WorkerService;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// RabbitMQ
var factory = new ConnectionFactory() { HostName = "localhost:6379" };
var connection = await factory.CreateConnectionAsync();
var channel = connection.CreateChannelAsync();

builder.Services.AddSingleton(channel);
builder.Services.AddSingleton(BidJsonContext.Default);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IQueue, Queue>();
builder.Services.AddSingleton<IRepository, Repository>();


var host = builder.Build();
host.Run();
