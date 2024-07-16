using EasyNetQ.Management.Client;
using RabbitMQMigrator.Factories;
using RabbitMQMigrator.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQMigrator;

public static class RabbitMQMigrator
{
    public static async Task<ComponentModel> GetComponents(ManagementClient client)
    {
        var exchangesTask = client.GetExchangesAsync();
        var queuesTask = client.GetQueuesAsync();
        var bindingsTask = client.GetBindingsAsync();

        await Task.WhenAll(queuesTask, exchangesTask, bindingsTask);

        return ComponentModelFactory.Create(exchangesTask.Result, queuesTask.Result, bindingsTask.Result);
    }

    public static async Task ApplySettings(ManagementClient client, ComponentModel components)
    {
        var tasks = new List<Task>();
        Logger.Log(LogType.Migrate_Exchanges_Start);
        var counter = 0;

        foreach (var exchange in components.Exchanges)
        {
            var exchangeInfo = ExchangeInfoFactory.Create(exchange);
            var exchangeTask = Task.Run(async () =>
            {
                try
                {
                    await client.CreateExchangeAsync(exchange.Vhost, exchangeInfo);
                    counter++;
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

        foreach (var queue in components.Queues)
        {
            var queueInfo = QueueInfoFactory.Create(queue);
            var queueTask = Task.Run(async () =>
            {
                try
                {
                    await client.CreateQueueAsync(queue.Vhost, queueInfo);
                    counter++;
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

        foreach (var binding in components.Bindings)
        {
            // we expect 2 possible DestinationType == "queue" or DestinationType == "exchange", log if not
            var bindingInfo = BindingInfoFactory.Create(binding);

            var bindingTask = Task.Run(async () =>
            {
                try
                {
                    if (binding.DestinationType == "queue")
                    {
                        await client.CreateQueueBindingAsync(binding.Vhost, binding.Source, binding.Destination, bindingInfo);
                        counter++;
                    }
                    else if (binding.DestinationType == "exchange")
                    {
                        await client.CreateExchangeBindingAsync(binding.Vhost, binding.Source, binding.Destination, bindingInfo);
                        counter++;
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
    }

    public static async Task DeleteSettings(ManagementClient client, ComponentModel components)
    {
        var tasks = new List<Task>();
        Logger.Log(LogType.Delete_Bindings_Start);
        var counter = 0;

        foreach (var binding in components.Bindings)
        {
            var bindingTask = Task.Run(async () =>
            {
                try
                {
                    await client.DeleteBindingAsync(binding);
                    counter++;
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

        foreach (var queue in components.Queues)
        {
            var queueTask = Task.Run(async () =>
            {
                try
                {
                    await client.DeleteQueueAsync(queue.Vhost, queue.Name);
                    counter++;
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

        foreach (var exchange in components.Exchanges)
        {
            var exchangeTask = Task.Run(async () =>
            {
                try
                {
                    await client.DeleteExchangeAsync(exchange.Vhost, exchange.Name);
                    counter++;
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
    }
}
