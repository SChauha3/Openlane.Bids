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
                using var command = new SqlCommand("INSERT INTO [Bids] ([AuctionId], [BidderName], [CarId], [Amount], [Timestamp], [TransactionId]) OUTPUT INSERTED.ID VALUES (@AuctionId, @BidderName, @CarId, @Amount, @Timestamp, @TransactionId)", connection);

                command.Parameters.AddWithValue("@AuctionId", bid.AuctionId);
                command.Parameters.AddWithValue("@BidderName", bid.BidderName);
                command.Parameters.AddWithValue("@CarId", bid.CarId);
                command.Parameters.AddWithValue("@Amount", bid.Amount);
                command.Parameters.AddWithValue("@Timestamp", bid.Timestamp);
                command.Parameters.AddWithValue("@TransactionId", bid.TransactionId);

                await connection.OpenAsync();
                var generatedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                _logger.LogInformation("Bid saved : {BidId}", generatedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bid could not stored. It will be retied later.");
                throw;
            }
        }

        public async Task<IEnumerable<Bid>> GetAsync(int auctionId, int carId, int cursor, int pageSize = 10)
        {
            var bids = new List<Bid>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetBid", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@AuctionId", auctionId);
            command.Parameters.AddWithValue("@CarId", carId);
            command.Parameters.AddWithValue("@Cursor", cursor);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            await connection.OpenAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    bids.Add(new Bid
                    {
                        Id = reader.GetInt32("Id"),
                        TransactionId = reader.GetGuid("TransactionId"),
                        AuctionId = reader.GetInt32("AuctionId"),
                        BidderName = reader.IsDBNull(reader.GetOrdinal("BidderName")) ? string.Empty : reader.GetString("BidderName"),
                        CarId = reader.GetInt32("CarId"),
                        Amount = reader.GetDecimal("Amount"),
                        Timestamp = reader.GetDateTime("Timestamp")
                    });
                }
            }

            return bids;
        }
    }
}
