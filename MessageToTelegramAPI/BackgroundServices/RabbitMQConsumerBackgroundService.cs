using MessageToTelegramAPI.Infra.Configurations;
using MessageToTelegramAPI.Infra.RabbitMQ.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageToTelegramAPI.BackgroundServices
{
    public class RabbitMQConsumerBackgroundService : BackgroundService
    {
        private ILogger<RabbitMQConsumerBackgroundService> _logger;
        private readonly IModel _model;
        private readonly MainApplicationQueueConfiguration _queueConfiguration;

        public RabbitMQConsumerBackgroundService(ILogger<RabbitMQConsumerBackgroundService> logger, IRabbitMQClient rabbitMQClient, IOptions<MainApplicationQueueConfiguration> options)
        {
            _logger = logger;
            _model = rabbitMQClient.Model;
            _queueConfiguration = options.Value;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => {
                _logger.LogDebug("Service is stopping.");
            });

            if(_model == null)
            {
                _logger.LogDebug("Service is stopping. Rabbit could not be connected.");
                return;
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                var consumer = new AsyncEventingBasicConsumer(_model);
                
                consumer.Received += Consumer_Received;
                consumer.Shutdown += Consumer_Shutdown;
                consumer.Registered += Consumer_Registered;
                consumer.Unregistered += Consumer_Unregistered;
                consumer.ConsumerCancelled += Consumer_ConsumerCancelled;

                _model.BasicConsume(_queueConfiguration.QueueName, true, consumer);
                await Task.CompletedTask;
            }
        }

        private async Task Consumer_ConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer cancelled: {e.ConsumerTags?.GetValue(0)}");
            await Task.CompletedTask;
        }

        private async Task Consumer_Unregistered(object sender, ConsumerEventArgs e)
        {

            _logger.LogInformation($"Consumer_Unregistered: {e.ConsumerTags?.GetValue(0)}");
            await Task.CompletedTask;
        }

        private async Task Consumer_Registered(object sender, ConsumerEventArgs e)
        {

            _logger.LogInformation($"Consumer_Registered: {e.ConsumerTags?.GetValue(0)}");
            await Task.CompletedTask;
        }

        private async Task Consumer_Shutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"Consumer_Shutdown: {e.ReplyText}");
            await Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var content = Encoding.UTF8.GetString(e.Body.Span);
                await HandleMessageAsync(content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message received but failed to consume:{ex.Message}");
            }
        }

        private async Task HandleMessageAsync(string content)
        {
            _logger.LogInformation($"Message received: {content}");
            await Task.CompletedTask;
        }
    }
}
