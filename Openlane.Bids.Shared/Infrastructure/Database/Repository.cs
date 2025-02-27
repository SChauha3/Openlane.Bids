﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Openlane.Bids.Shared.Models;
using System.Data;

namespace Openlane.Bids.Shared.Infrastructure.Database
{
    public class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private readonly string _connectionString;
        public Repository(ILogger<Repository> logger, string connectionString) 
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        public async Task SaveAsync(Bid bid)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SaveBid", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Id", bid.Id);
            command.Parameters.AddWithValue("@AuctionId", bid.AuctionId);
            command.Parameters.AddWithValue("@CarId", bid.CarId);
            command.Parameters.AddWithValue("@Amount", bid.Amount);
            command.Parameters.AddWithValue("@Timestamp", bid.Timestamp);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            _logger.LogInformation("Bid saved to SQL Server: {BidId}", bid.Id);
        }

        public async Task<IEnumerable<Bid>> GetAsync(int auctionId, int pageSize, int cursor)
        {
            var bids = new List<Bid>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("GetBid", connection);
                command.Parameters.AddWithValue("@AuctionId", auctionId);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Cursor", (object)cursor ?? DBNull.Value);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        bids.Add(new Bid
                        {
                            Id = reader.GetInt32("Id"),
                            AuctionId = reader.GetInt32("AuctionId"),
                            CarId = reader.GetInt32("CarId"),
                            Amount = reader.GetDecimal("Amount"),
                            Timestamp = reader.GetDateTime("Timestamp")
                        });
                    }
                }
            }
            return bids;
        }
    }
}
