using Microsoft.Data.SqlClient;
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
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("INSERT INTO [Bids] ([AuctionId], [CarId], [Amount], [Timestamp]) OUTPUT INSERTED.ID VALUES (@AuctionId, @CarId, @Amount, @Timestamp)", connection);

                command.Parameters.AddWithValue("@AuctionId", bid.AuctionId);
                command.Parameters.AddWithValue("@CarId", bid.CarId);
                command.Parameters.AddWithValue("@Amount", bid.Amount);
                command.Parameters.AddWithValue("@Timestamp", bid.Timestamp);

                await connection.OpenAsync();
                var generatedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                _logger.LogInformation("Bid saved : {BidId}", generatedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
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
