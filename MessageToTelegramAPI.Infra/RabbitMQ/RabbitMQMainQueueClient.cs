using MessageToTelegramAPI.Infra.Configurations;
using MessageToTelegramAPI.Infra.RabbitMQ.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace MessageToTelegramAPI.Infra.RabbitMQ
{
    public class RabbitMQMainQueueClient: IRabbitMQClient
    {
        private readonly IModel _model;
        private readonly MainApplicationQueueConfiguration _queueConfiguration;

        public RabbitMQMainQueueClient(IRabbitMQContext rabbitMQContext, IOptions<MainApplicationQueueConfiguration> options)
        {
            _queueConfiguration = options.Value;
            _model = rabbitMQContext.Connection.CreateModel();
            _model.ExchangeDeclare(_queueConfiguration.ExchangeName, _queueConfiguration.ExchangeType, _queueConfiguration.Durable, autoDelete: false);
            _model.QueueDeclare(_queueConfiguration.QueueName, _queueConfiguration.Durable, _queueConfiguration.Exclusive, _queueConfiguration.AutoDelete);
            _model.QueueBind(_queueConfiguration.QueueName, _queueConfiguration.ExchangeName, _queueConfiguration.RoutingKey);
        }

        IModel IRabbitMQClient.Model => _model;

        

        public async Task SendMessage(string message)
        {
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(message);
            IBasicProperties props = _model.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            _model.BasicPublish(_queueConfiguration.ExchangeName, _queueConfiguration.RoutingKey, props, messageBodyBytes);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }
}
