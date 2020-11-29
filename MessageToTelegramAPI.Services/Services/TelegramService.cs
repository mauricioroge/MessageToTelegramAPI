using MessageToTelegramAPI.Domain.Entities;
using MessageToTelegramAPI.Domain.Services;
using MessageToTelegramAPI.Infra.RabbitMQ.Interfaces;
using System.Text.Json;

namespace MessageToTelegramAPI.Services.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly IUserToServerMQClient _userToServerClient;
        private readonly IServerToUserMQClient _serverToUserMQClient;

        public TelegramService(IUserToServerMQClient userToServerClient,
                               IServerToUserMQClient serverToUserMQClient)
        {
            _userToServerClient = userToServerClient;
            _serverToUserMQClient = serverToUserMQClient;
        }

        public void SendMessageFromUserToServer(UserMessage message)
        {
            if (message != null)
            {
                var json = JsonSerializer.Serialize(message);
                //The desired way
                //_userToServerClient.SendMessage(json);

                //This is only for tests purposes
                _serverToUserMQClient.SendMessage(json);
            }
        }
    }
}
