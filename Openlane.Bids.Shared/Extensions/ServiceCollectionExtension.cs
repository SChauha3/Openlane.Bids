using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public static async Task<IServiceCollection> AddQueueService(this IServiceCollection services)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost", // Set hostname only
                Port = 5672, // Default RabbitMQ port
                UserName = "user", // Add username if needed
                Password = "password" // Add password if needed
            };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            services.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IQueueService<Bid>>>();
                return new QueueService(logger, channel);
            });

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services)
        {
            var configuration = ConfigurationOptions.Parse("localhost:6379", true);
            configuration.AbortOnConnectFail = true;
            var connectionMultiplexer = ConnectionMultiplexer.Connect(configuration);
            var redisDatabase = connectionMultiplexer.GetDatabase();

            services.AddSingleton(sp =>
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
                return new Repository(logger, "YourAdditionalParameter");
            });

            return services;
        }
    }
}
