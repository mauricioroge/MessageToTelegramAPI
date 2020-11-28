using MessageToTelegramAPI.Infra.Configurations;
using MessageToTelegramAPI.Infra.RabbitMQ.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace MessageToTelegramAPI.Infra.RabbitMQ
{
    public class RabbitMQContext : IRabbitMQContext
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMQContext> _logger;
        private readonly RabbitMQConfiguration _configuration;
        

        public RabbitMQContext(ILogger<RabbitMQContext> logger, IOptions<RabbitMQConfiguration> configuration)
        {
            _logger = logger;
            _configuration = configuration.Value;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration.HostName,
                    UserName = _configuration.Username,
                    Password = _configuration.Secret,
                    VirtualHost = _configuration.VirtualHost,
                    DispatchConsumersAsync = _configuration.DispatcherAsync,
                    TopologyRecoveryEnabled = _configuration.TopologyRecovery,
                    AutomaticRecoveryEnabled = _configuration.AutomaticRecovery,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(_configuration.NetworkRecoverySeconds)
                };
                _connection = factory.CreateConnection();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IConnection Connection => _connection;

        public void Dispose()
        {
            if (_connection?.IsOpen ?? false)
            {
                _connection.Close();
            }
        }
    }
}
