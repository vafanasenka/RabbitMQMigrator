using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;
using RabbitMQMigrator.Factories;
using RabbitMQMigrator.Models;
using System.Collections.Generic;
using System.Linq;
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

    public static async Task<IEnumerable<Binding>> ApplySettings(ManagementClient client, ComponentModel components)
    {
        var tasks = Enumerable.Empty<Task>();

        foreach (var exchange in components.Exchanges)
        {
            var exchangeInfo = ExchangeInfoFactory.Create(exchange);
            var exchangeTask = client.CreateExchangeAsync(exchange.Vhost, exchangeInfo);
            _ = tasks.Append(exchangeTask);
        }

        foreach (var queue in components.Queues)
        {
            var queueInfo = QueueInfoFactory.Create(queue);
            var queueTask = client.CreateQueueAsync(queue.Vhost, queueInfo);
            _ = tasks.Append(queueTask);
        }

        await Task.WhenAll(tasks);

        tasks = [];
        var errorBindings = Enumerable.Empty<Binding>();

        foreach (var binding in components.Bindings)
        {
            // we expect 2 possible DestinationType == "queue" or DestinationType == "exchange", log if not
            var bindingInfo = BindingInfoFactory.Create(binding);
            if (binding.DestinationType == "queue")
            {
                var bindingTask = client.CreateQueueBindingAsync(binding.Vhost, binding.Source, binding.Destination, bindingInfo);
                _ = tasks.Append(bindingTask);
            } else if (binding.DestinationType == "exchange")
            {
                var bindingTask = client.CreateExchangeBindingAsync(binding.Vhost, binding.Source, binding.Destination, bindingInfo);
                _ = tasks.Append(bindingTask);
            } else
            {
                _ = errorBindings.Append(binding);
            }
        }

        await Task.WhenAll(tasks);

        return errorBindings;
    }
}
