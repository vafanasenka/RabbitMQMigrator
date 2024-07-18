using EasyNetQ.Management.Client;
using RabbitMQMigrator.Factories;
using RabbitMQMigrator.Loggers;
using RabbitMQMigrator.Migrators;
using RabbitMQMigrator.Providers;
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
        using var targetClient = new ManagementClient(new Uri($"http://{targetServer.HostName}:{targetServer.ManagementPort}"), targetServer.UserName, targetServer.Password);
        var settingMigrator = new SettingMigrator(sourceClient, targetClient);
        
        var settings = await settingMigrator.GetSettings();

        // for test and log purposes
        SettingsLogger.Log(settings);

        if (KeyHandler.WaitingForYNKeyPress("Press 'Y' to DELETE previously imported Settings from Target server or 'N' to continue..."))
        {
            await settingMigrator.DeleteSettings(settings);
        }

        if (KeyHandler.WaitingForYNKeyPress("Press 'Y' to APPLY settings to the Target server or 'N' to continue..."))
        {
            await settingMigrator.ApplySettings(settings);
        }

        if (KeyHandler.WaitingForYNKeyPress("Press 'Y' to start migrating MESSAGES or 'N' to continue..."))
        {
            var messageMigrator = new MessageMigrator(sourceConnection, targetConnection, settings.Queues);
            await messageMigrator.Migrate();
        }

        Logger.Log(LogType.Done, "Process completed.");
    }
}
