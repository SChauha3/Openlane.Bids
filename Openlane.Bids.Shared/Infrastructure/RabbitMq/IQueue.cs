using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlane.Bids.Shared.Infrastructure.RabbitMq
{
    public interface IQueue
    {
        Task PublishAsync(string message);
    }
}
