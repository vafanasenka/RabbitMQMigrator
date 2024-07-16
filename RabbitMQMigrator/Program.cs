using EasyNetQ.Management.Client;
using RabbitMQMigrator.Factories;
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

        //Console.WriteLine($"Source HostName & Ports (management/AMQP): {sourceServer.HostName} {sourceServer.ManagementPort}/{sourceServer.AMQPPort}, UserName: {sourceServer.UserName}; Password: {sourceServer.Password}");
        //Console.WriteLine($"Target HostName & Ports (management/AMQP): {targetServer.HostName} {targetServer.ManagementPort}/{targetServer.AMQPPort}, UserName: {targetServer.UserName}; Password: {targetServer.Password}");

        Console.WriteLine($"Log file path: {Logger.Initialize()}");

        Console.WriteLine("Press any key to connect to Source and Target servers...");
        Console.ReadKey();

        Logger.Log(LogType.Connect, "Connecting to Source server...");
        // AMQP connection
        using var sourceConnection = RabbitMQConnector.Connect(sourceServer.HostName, sourceServer.AMQPPort, sourceServer.UserName, sourceServer.Password);
        Logger.Log(LogType.Connected);

        Logger.Log(LogType.Connect, "Connecting to Target server...");
        // AMQP connection
        using var targetConnection = RabbitMQConnector.Connect(targetServer.HostName, targetServer.AMQPPort, targetServer.UserName, targetServer.Password);
        Logger.Log(LogType.Connected);

        Console.WriteLine("Press any key to fetch components from the Source server...");
        Console.ReadKey();

        Logger.Log(LogType.Get_Components_Start, "Fetching components from Source server...");
        using var sourceClient = new ManagementClient(new Uri($"http://{sourceServer.HostName}:{sourceServer.ManagementPort}"), sourceServer.UserName, sourceServer.Password);
        var components = await RabbitMQMigrator.GetComponents(sourceClient);
        Logger.Log(LogType.Get_Components_Done);

        // print just for test and log purposes
        Logger.Log(LogType.Log_Components_Start, "Log components from Source server...");
        DataLogger.Log(components);
        Logger.Log(LogType.Log_Components_Done);

        using var targetClient = new ManagementClient(new Uri($"http://{targetServer.HostName}:{targetServer.ManagementPort}"), targetServer.UserName, targetServer.Password);

        if (WaitForKeyPress("Press 'Y' to DELETE settings from Target server or 'N' to continue..."))
        {
            Logger.Log(LogType.Delete_Settings_Start, "Delete settings from Target server...");
            await RabbitMQMigrator.DeleteSettings(targetClient, components);
            Logger.Log(LogType.Delete_Settings_Done);
        }

        if (WaitForKeyPress("Press 'Y' to APPLY settings to the Target server or 'N' to continue..."))
        {
            Logger.Log(LogType.Create_Settings_Start, "Applying settings to Target server...");
            await RabbitMQMigrator.ApplySettings(targetClient, components);
            Logger.Log(LogType.Create_Settings_Done);
        }

        // redevelop if we need to migrate messages
        /*
        Console.WriteLine("Press any key to migrate messages...");
        Console.ReadKey();

        foreach (var queue in (JArray)settings["queues"])
        {
            var queueName = (string)queue["name"];
            Logger.Log(LogType.Migrate_Messages_Start, $"Migrating messages from queue {queueName}...");
            MessageMigrator.MigrateMessages(sourceConnection, targetConnection, queueName);
            Logger.Log(LogType.Migrate_Messages_Done, $"Migrated messages from queue {queueName}.");
        }
        */

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
