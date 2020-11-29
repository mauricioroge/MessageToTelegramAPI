namespace MessageToTelegramAPI.Infra.Configurations
{
    public class UserMessagesQueueConfiguration
    {

        public string QueueName { get; set; }
        public bool Durable { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeType { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }


    }
}
