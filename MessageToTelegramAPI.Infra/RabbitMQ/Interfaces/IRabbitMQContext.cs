using RabbitMQ.Client;
using System;

namespace MessageToTelegramAPI.Infra.RabbitMQ.Interfaces
{
    public interface IRabbitMQContext : IDisposable
    {
        IConnection Connection { get; }
    }
}
