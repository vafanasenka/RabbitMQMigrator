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

        // TODO check applaying if get works
        /*
        Logger.Log(LogType.Connect, "Connecting to Target server...");
        // AMQP connection
        using var targetConnection = RabbitMQConnector.Connect(targetHostName, targetAMQPPort, targetUserName, targetPassword);
        Logger.Log(LogType.Connected);
        */

        Console.WriteLine("Press any key to fetch settings from the Source server...");
        Console.ReadKey();

        Logger.Log(LogType.Get_Settings_Start, "Fetching settings from Source server...");
        using var sourceHttpClient = new RabbitMQHttpClient($"http://{sourceServer.HostName}:{sourceServer.ManagementPort}", sourceServer.UserName, sourceServer.Password);
        var settings = await RabbitMQMigrator.GetSettings(sourceHttpClient);
        Logger.Log(LogType.Get_Settings_Done);

        // print just for test and log purposes
        Logger.Log(LogType.Print_Settings_Start, "Print settings from Source server...");
        SettingsPrinter.Print(settings);
        Logger.Log(LogType.Print_Settings_Done);

        // TODO check applaying if get works
        /*
        Console.WriteLine("Press any key to apply settings to the Target server...");
        Console.ReadKey();

        Logger.Log(LogType.Create_Settings_Start, "Applying settings to Target server...");
        using var targetHttpClient = new RabbitMQHttpClient($"http://{targetHostName}:{targetManagementPort}", targetUserName, targetPassword);
        await RabbitMQMigrator.ApplySettings(targetHttpClient, settings);
        Logger.Log(LogType.Create_Settings_Done);
        */

        // I think we don't need to do attempt to migrate messages
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

        Logger.Log(LogType.Done, "Migration completed.");
        Console.WriteLine("Migration completed.");
    }
}
