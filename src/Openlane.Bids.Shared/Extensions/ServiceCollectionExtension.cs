using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Openlane.Bids.Shared.Infrastructure;
using Openlane.Bids.Shared.Infrastructure.Database;
using Openlane.Bids.Shared.Infrastructure.Services.Caches;
using Openlane.Bids.Shared.Infrastructure.Services.Queues;
using Openlane.Bids.Shared.Models;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Openlane.Bids.Shared.Extensions
{
    public static class ServiceCollectionExtension
    {
        private static InfraSettings _infraSettings;
        public static void Initialize(InfraSettings infraSettings)
        {
            _infraSettings = infraSettings;
        }
        public static async Task<IServiceCollection> AddQueueService(this IServiceCollection services)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port =  5672,
                UserName = "user",
                Password = "password"
            };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "openlane-bids", type: "direct", durable:true);
            await channel.QueueDeclareAsync(queue: "bids-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync(queue: "bids-queue", exchange: "openlane-bids", routingKey: "openlane.bid.creation");

            services.AddSingleton<IQueueService<BidEvent>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IQueueService<BidEvent>>>();

                return new QueueService(logger, channel);
            });

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services)
        {
            var configuration = new ConfigurationOptions
            {
                EndPoints = { "redis:6379" },
                Password = "Strong!Pass123",
                ConnectTimeout = 5000,
                AbortOnConnectFail = false
            };
            configuration.AbortOnConnectFail = false;
            var connectionMultiplexer = ConnectionMultiplexer.Connect(configuration);
            var redisDatabase = connectionMultiplexer.GetDatabase();

            services.AddSingleton<ICacheService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ICacheService>>();
                return new CacheService(logger, redisDatabase);
            });
            return services;
        }

        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddSingleton<IRepository>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<Repository>>();
                
                return new Repository(logger, "Server=sqlserver-db;Database=OpenlaneDb;User Id=sa;Password=StrongP@ssw0rd123;TrustServerCertificate=True;");
                //return new Repository(logger, "Data Source=INDU-PC\\SQLEXPRESS;Initial Catalog=Openlane_Bids;Integrated Security=True;Encrypt=False");
            });

            return services;
        }
    }
}