namespace Hangfire.Server
{
    public class MassTransitConfigEntities
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool SSLActive { get; set; }

    }
}
