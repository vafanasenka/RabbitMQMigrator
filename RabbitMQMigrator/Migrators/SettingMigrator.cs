using EasyNetQ.Management.Client;
using RabbitMQMigrator.Factories;
using RabbitMQMigrator.Loggers;
using RabbitMQMigrator.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQMigrator.Migrators;

public class SettingMigrator
{
    private readonly ManagementClient sourceClient;
    private readonly ManagementClient targetClient;

    public SettingMigrator(ManagementClient sourceClient, ManagementClient targetClient)
    {
        ArgumentNullException.ThrowIfNull(sourceClient, nameof(sourceClient));
        ArgumentNullException.ThrowIfNull(targetClient, nameof(targetClient));

        this.sourceClient = sourceClient;
        this.targetClient = targetClient;
    }

    public async Task<SettingModel> GetSettings()
    {
        Logger.Log(LogType.Get_Settings_Start, "Fetching Settings from Source server...");

        var exchangesTask = sourceClient.GetExchangesAsync();
        var queuesTask = sourceClient.GetQueuesAsync();
        var bindingsTask = sourceClient.GetBindingsAsync();

        await Task.WhenAll(queuesTask, exchangesTask, bindingsTask);

        var settings = SettingModelFactory.Create(exchangesTask.Result, queuesTask.Result, bindingsTask.Result);
        Logger.Log(LogType.Get_Settings_Done);
        return settings;
    }

    public async Task ApplySettings(SettingModel settings)
    {
        var tasks = new List<Task>();
        Logger.Log(LogType.Create_Settings_Start, "Applying Settings to Target server...");
        Logger.Log(LogType.Migrate_Exchanges_Start);
        var counter = 0;

        foreach (var exchange in settings.Exchanges)
        {
            var exchangeInfo = ExchangeInfoFactory.Create(exchange);
            var exchangeTask = Task.Run(async () =>
            {
                try
                {
                    await targetClient.CreateExchangeAsync(exchange.Vhost, exchangeInfo);
                    counter += 1;
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.Exception, $"Failed to create exchange: {exchange.Name}. Error: {e.Message}");
                }
            });

            tasks.Add(exchangeTask);
        }

        Logger.Log(LogType.Migrate_Exchanges_Start, $"Exchages to migrate count: {counter}");
        await Task.WhenAll(tasks);
        Logger.Log(LogType.Migrate_Exchanges_Done);

        tasks.Clear();
        Logger.Log(LogType.Migrate_Queues_Start);
        counter = 0;

        foreach (var queue in settings.Queues)
        {
            var queueInfo = QueueInfoFactory.Create(queue);
            var queueTask = Task.Run(async () =>
            {
                try
                {
                    await targetClient.CreateQueueAsync(queue.Vhost, queueInfo);
                    counter += 1;
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.Exception, $"Failed to create queue: {queue.Name}. Error: {e.Message}");
                }
            });

            tasks.Add(queueTask);
        }

        Logger.Log(LogType.Migrate_Queues_Start, $"Queues to migrate count: {counter}");
        await Task.WhenAll(tasks);
        Logger.Log(LogType.Migrate_Queues_Done);

        tasks.Clear();
        Logger.Log(LogType.Migrate_Bindings_Start);
        counter = 0;

        foreach (var binding in settings.Bindings)
        {
            // we expect 2 possible DestinationType == "queue" or DestinationType == "exchange", log if not
            var bindingInfo = BindingInfoFactory.Create(binding);

            var bindingTask = Task.Run(async () =>
            {
                try
                {
                    if (binding.DestinationType == "queue")
                    {
                        await targetClient.CreateQueueBindingAsync(binding.Vhost, binding.Source, binding.Destination, bindingInfo);
                        counter += 1;
                    }
                    else if (binding.DestinationType == "exchange")
                    {
                        await targetClient.CreateExchangeBindingAsync(binding.Vhost, binding.Source, binding.Destination, bindingInfo);
                        counter += 1;
                    }
                    else
                    {
                        Logger.Log(LogType.Error, $"Failed to create binding: {binding.DestinationType} is not handled");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.Exception, $"Failed to create binding: {binding.Source} -> {binding.Destination}. Error: {e.Message}");
                }
            });

            tasks.Add(bindingTask);
        }

        Logger.Log(LogType.Migrate_Bindings_Start, $"Bindings to migrate count: {counter}");
        await Task.WhenAll(tasks);
        Logger.Log(LogType.Migrate_Bindings_Done);
        Logger.Log(LogType.Create_Settings_Done);
    }

    public async Task DeleteSettings(SettingModel settings)
    {
        var tasks = new List<Task>();
        Logger.Log(LogType.Delete_Settings_Start, "Delete Settings from Target server...");
        Logger.Log(LogType.Delete_Bindings_Start);
        var counter = 0;

        foreach (var binding in settings.Bindings)
        {
            var bindingTask = Task.Run(async () =>
            {
                try
                {
                    await targetClient.DeleteBindingAsync(binding);
                    counter += 1;
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.Exception, $"Failed to delete binding: {binding.Source} -> {binding.Destination}. Error: {e.Message}");
                }
            });

            tasks.Add(bindingTask);
        }

        Logger.Log(LogType.Delete_Bindings_Start, $"Bindings to delete count: {counter}");
        await Task.WhenAll(tasks);
        Logger.Log(LogType.Delete_Bindings_Done);

        tasks.Clear();
        Logger.Log(LogType.Delete_Queues_Start);
        counter = 0;

        foreach (var queue in settings.Queues)
        {
            var queueTask = Task.Run(async () =>
            {
                try
                {
                    await targetClient.DeleteQueueAsync(queue.Vhost, queue.Name);
                    counter += 1;
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.Exception, $"Failed to delete queue: {queue.Name}. Error: {e.Message}");
                }
            });

            tasks.Add(queueTask);
        }

        Logger.Log(LogType.Delete_Queues_Start, $"Queues to delete count: {counter}");
        await Task.WhenAll(tasks);
        Logger.Log(LogType.Delete_Queues_Done);

        tasks.Clear();
        Logger.Log(LogType.Delete_Exchanges_Start);
        counter = 0;

        foreach (var exchange in settings.Exchanges)
        {
            var exchangeTask = Task.Run(async () =>
            {
                try
                {
                    await targetClient.DeleteExchangeAsync(exchange.Vhost, exchange.Name);
                    counter += 1;
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.Exception, $"Failed to delete exchange: {exchange.Name}. Error: {e.Message}");
                }
            });

            tasks.Add(exchangeTask);
        }

        Logger.Log(LogType.Delete_Exchanges_Start, $"Exchages to delete count: {counter}");
        await Task.WhenAll(tasks);
        Logger.Log(LogType.Delete_Exchanges_Done);
        Logger.Log(LogType.Delete_Settings_Done);
    }
}
