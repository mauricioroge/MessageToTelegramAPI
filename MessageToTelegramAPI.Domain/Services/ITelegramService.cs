using MessageToTelegramAPI.Domain.Entities;

namespace MessageToTelegramAPI.Domain.Services
{
    public interface ITelegramService
    {
        /// <summary>
        /// Send the message received from USER to a Server where it'll process the message
        /// </summary>
        /// <param name="message">It must contains an instance of UserMessage</param>
        void SendMessageFromUserToServer(UserMessage message);

    }
}
