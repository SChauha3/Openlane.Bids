using Openlane.Bids.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Openlane.Bids.Shared
{
    [JsonSerializable(typeof(Bid))]
    public partial class BidJsonContext : JsonSerializerContext
    {
    }
}
