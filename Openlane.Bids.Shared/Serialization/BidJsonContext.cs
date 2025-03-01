using Openlane.Bids.Shared.Models;
using System.Text.Json.Serialization;

namespace Openlane.Bids
{
    [JsonSerializable(typeof(Bid))]
    [JsonSerializable(typeof(IEnumerable<Bid>))]
    public partial class BidJsonContext : JsonSerializerContext
    {
    }
}
