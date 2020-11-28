namespace MessageToTelegramAPI.Infra.Configurations
{
    public class RabbitMQConfiguration
    {
        public string Username { get; set; }
        public string Secret { get; set; }
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public bool DispatcherAsync { get; set; }
        public bool TopologyRecovery { get; set; }
        public bool AutomaticRecovery { get; set; }
        public int NetworkRecoverySeconds { get; set; }
    }
}
