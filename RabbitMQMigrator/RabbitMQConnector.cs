using RabbitMQ.Client;
using RabbitMQMigrator.Models;
using System;

namespace RabbitMQMigrator;

public static class RabbitMQConnector
{
    public static IConnection Connect(ServerModel server)
    {
        ArgumentNullException.ThrowIfNull(server, nameof(server));

        return DoConnect(server);
    }

    private static IConnection DoConnect(ServerModel server)
    {
        Logger.Log(LogType.Connect, $"Connecting to {server.HostName}...");

        var connectionFactory = new ConnectionFactory
        {
            HostName = server.HostName,
            Port = server.AMQPPort,
            UserName = server.UserName,
            Password = server.Password
        };

        var connection = connectionFactory.CreateConnection();
        Logger.Log(LogType.Connected);

        return connection;
    }

    public static void CloseConnection(IConnection connection)
    {
        if (connection != null && connection.IsOpen)
        {
            connection.Close();
        }
    }
}
