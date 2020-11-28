using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace MessageToTelegramAPI.Infra.RabbitMQ.Interfaces
{
    public interface IRabbitMQClient: IDisposable
    {
        IModel Model { get; }
        Task SendMessage(string message);
    }
}
