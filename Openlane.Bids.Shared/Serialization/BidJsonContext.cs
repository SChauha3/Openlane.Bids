using Openlane.Bids.Shared.Models;
using System.Text.Json.Serialization;

namespace Openlane.Bids
{
    [JsonSerializable(typeof(Bid))]
    [JsonSerializable(typeof(IEnumerable<Bid>))]
    [JsonSerializable(typeof(BidEvent))]
    [JsonSerializable(typeof(IEnumerable<BidEvent>))]
    public partial class BidJsonContext : JsonSerializerContext
    {
    }
}
