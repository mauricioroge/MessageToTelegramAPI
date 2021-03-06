﻿using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace MessageToTelegramAPI.Infra.RabbitMQ.Interfaces
{
    public interface IUserToServerMQClient: IDisposable
    {
        IModel Model { get; }
        Task SendMessage(string message);
    }
}
