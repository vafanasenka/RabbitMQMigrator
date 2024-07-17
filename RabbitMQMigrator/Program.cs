using EasyNetQ.Management.Client;
using RabbitMQMigrator.Factories;
using RabbitMQMigrator.Migrator;
using System;
using System.Threading.Tasks;

namespace RabbitMQMigrator;

public class Program
{
    static async Task Main(string[] args)
    {
        var sourceConfig = ConfigurationProvider.GetConfiguration(Constants.Config.SourceServer);
        var targetConfig = ConfigurationProvider.GetConfiguration(Constants.Config.TargetServer);

        var sourceServer = ServerModelFactory.Create(sourceConfig);
        var targetServer = ServerModelFactory.Create(targetConfig);

        Console.WriteLine($"Log file path: {Logger.Initialize()}");

        Console.WriteLine("Press any key to connect to Source and Target servers...");
        Console.ReadKey();

        // AMQP connection
        using var sourceConnection = RabbitMQConnector.Connect(sourceServer);
        using var targetConnection = RabbitMQConnector.Connect(targetServer);

        Console.WriteLine("Press any key to fetch Settings from the Source server...");
        Console.ReadKey();

        using var sourceClient = new ManagementClient(new Uri($"http://{sourceServer.HostName}:{sourceServer.ManagementPort}"), sourceServer.UserName, sourceServer.Password);
        var settings = await RabbitMQMigrator.GetSettings(sourceClient);

        // for test and log purposes
        DataLogger.Log(settings);

        using var targetClient = new ManagementClient(new Uri($"http://{targetServer.HostName}:{targetServer.ManagementPort}"), targetServer.UserName, targetServer.Password);

        if (WaitForKeyPress("Press 'Y' to DELETE previously imported Settings from Target server or 'N' to continue..."))
        {
            await RabbitMQMigrator.DeleteSettings(targetClient, settings);
        }

        if (WaitForKeyPress("Press 'Y' to APPLY settings to the Target server or 'N' to continue..."))
        {
            await RabbitMQMigrator.ApplySettings(targetClient, settings);
        }

        if (WaitForKeyPress("Press 'Y' to start migrating messages or 'N' to continue..."))
        {
            var migrator = new MessageMigrator(sourceConnection, targetConnection, settings.Queues);
            await migrator.Migrate();
        }

        Logger.Log(LogType.Done, "Process completed.");
    }

    static bool WaitForKeyPress(string message)
    {
        Console.WriteLine($"{message} (Y/N)");
        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Y)
            {
                return true;
            }
            else if (key == ConsoleKey.N)
            {
                return false;
            }
        }
    }
}
