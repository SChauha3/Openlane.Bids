namespace Openlane.Bids.Api.Models
{
    public class Auction
    {
        public int CarId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string CarMake { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
