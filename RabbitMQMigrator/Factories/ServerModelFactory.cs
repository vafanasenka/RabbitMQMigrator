using Newtonsoft.Json.Linq;
using System;

namespace RabbitMQMigrator.Factories;

public static class ServerModelFactory
{
    public static ServerModel Create(JToken configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return DoCreate(configuration);
    }

    private static ServerModel DoCreate(JToken configuration) => new()
    {
        HostName = (string)configuration[Constants.Server.HostName],
        ManagementPort = (int)configuration[Constants.Server.ManagementPort],
        AMQPPort = (int)configuration[Constants.Server.AMQPPort],
        UserName = (string)configuration[Constants.Server.UserName],
        Password = (string)configuration[Constants.Server.Password]
    };
}
