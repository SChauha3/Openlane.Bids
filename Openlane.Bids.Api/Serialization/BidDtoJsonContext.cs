using Openlane.Bids.Shared.Dtos;
using System.Text.Json.Serialization;

namespace Openlane.Bids.Serialization
{
    [JsonSerializable(typeof(Bid))]
    public partial class BidDtoJsonContext : JsonSerializerContext
    {
    }
}
