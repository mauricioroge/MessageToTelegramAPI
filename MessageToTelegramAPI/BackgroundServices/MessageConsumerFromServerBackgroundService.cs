using MessageToTelegramAPI.Domain.Entities;
using MessageToTelegramAPI.Domain.Services;
using MessageToTelegramAPI.Infra.Configurations;
using MessageToTelegramAPI.Infra.RabbitMQ.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageToTelegramAPI.BackgroundServices
{
    public class MessageConsumerFromServerBackgroundService : BackgroundService
    {
        private ILogger<MessageConsumerFromServerBackgroundService> _logger;
        private readonly IModel _model;
        private readonly ServerMessagesQueueConfiguration _queueConfiguration;
        private readonly ITelegramService _telegramService;
        private static ITelegramBotClient botClient;

        public MessageConsumerFromServerBackgroundService(ILogger<MessageConsumerFromServerBackgroundService> logger,
                                                 IServerToUserMQClient rabbitMQClient,
                                                 IOptions<ServerMessagesQueueConfiguration> queueOpt,
                                                 ITelegramService telegramService,
                                                 IOptions<TelegramBotConfiguration> telegramOpt)
        {
            _logger = logger;
            _model = rabbitMQClient.Model;
            _queueConfiguration = queueOpt.Value;
            _telegramService = telegramService;
            botClient = new TelegramBotClient(telegramOpt.Value.Token);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                _logger.LogDebug("Service is stopping.");
            });

            if (_model == null)
            {
                _logger.LogDebug("Service is stopping. Rabbit could not be connected.");
                return;
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                #region Telegram Bot

                var botMe = botClient.GetMeAsync(stoppingToken).Result;
                _logger.LogInformation($"Hello, World! I am user {botMe.Id} and my name is {botMe.FirstName}.");

                botClient.OnMessage += Bot_OnMessage;
                botClient.StartReceiving(cancellationToken: stoppingToken);

                #endregion


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
                var serverMessage = JsonSerializer.Deserialize<ServerMessage>(content);

                await HandleMessage(serverMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message received but failed to consume:{ex.Message}");
            }
        }

        private static async Task HandleMessage(ServerMessage content)
        {
            await botClient.SendTextMessageAsync(
                    chatId: content.ChatId,
                    text: content.Message,
                    parseMode: ParseMode.Markdown,
                    replyToMessageId: content.MessageId);
            
        }
        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                _logger.LogInformation($"Received a text message in chat {e.Message.Chat.Id}.");

                UserMessage userMessage = new UserMessage()
                {
                    ChatId = e.Message.Chat.Id,
                    Message = e.Message.Text,
                    MessageId = e.Message.MessageId,
                    Created_at = DateTime.UtcNow,
                    Sent_at = e.Message.Date
                };

                _telegramService.SendMessageFromUserToServer(userMessage);

            }
            await Task.CompletedTask;
        }
    }
}
