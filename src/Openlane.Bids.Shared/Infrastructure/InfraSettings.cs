namespace Openlane.Bids.Shared.Infrastructure
{
    public class InfraSettings
    {
        public CacheSettings CacheSettings { get; set; } = new CacheSettings();
        public QueueSettings QueueSettings { get; set; } = new QueueSettings();
        public DbSettings DbSettings { get; set; } = new DbSettings();
    }

    public class CacheSettings
    {
        public string Endpoints { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class QueueSettings
    {
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class DbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
}
