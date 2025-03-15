using Openlane.Bids.Shared.Infrastructure;

namespace Openlane.Bids.Api
{
    public class AppSettings
    {
        public InfraSettings InfraSettings { get; set; } = new InfraSettings();
    }
}
