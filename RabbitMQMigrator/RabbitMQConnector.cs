using RabbitMQ.Client;

namespace RabbitMQMigrator;

public static class RabbitMQConnector
{
    public static IConnection Connect(string hostName, int port, string userName, string password)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password
        };

        return connectionFactory.CreateConnection();
    }

    public static void CloseConnection(IConnection connection)
    {
        if (connection != null && connection.IsOpen)
        {
            connection.Close();
        }
    }
}
