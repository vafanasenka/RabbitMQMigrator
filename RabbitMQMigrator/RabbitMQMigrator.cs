using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;
using RabbitMQMigrator.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQMigrator;

public static class RabbitMQMigrator
{
    public static async Task<Dictionary<string, object>> GetSettings(ManagementClient client)
    {
        // TODO exclude exchanges with 'default' user:
        // "user_who_performed_action": "rmq-internal"
        var exchangesTask = client.GetExchangesAsync();
        var queuesTask = client.GetQueuesAsync();
        var bindingsTask = client.GetBindingsAsync();

        await Task.WhenAll(queuesTask, exchangesTask, bindingsTask);

        var settings = new Dictionary<string, object>
        {
            { Constants.Settings.ExchangesKey, exchangesTask.Result },
            { Constants.Settings.QueuesKey, queuesTask.Result },
            { Constants.Settings.BindingsKey, bindingsTask.Result }
        };
        
        return settings;
    }

    public static async Task<IEnumerable<Binding>> ApplySettings(ManagementClient client, Dictionary<string, object> settings)
    {
        var exchanges = (IEnumerable<Exchange>)settings[Constants.Settings.ExchangesKey];
        var queues = (IEnumerable<Queue>)settings[Constants.Settings.QueuesKey];
        var bindings = (IEnumerable<Binding>)settings[Constants.Settings.BindingsKey];

        var tasks = Enumerable.Empty<Task>();

        foreach (var exchange in exchanges)
        {
            var exchangeInfo = ExchangeInfoFactory.Create(exchange);
            var exchangeTask = client.CreateExchangeAsync(exchange.Vhost, exchangeInfo);
            _ = tasks.Append(exchangeTask);
        }

        foreach (var queue in queues)
        {
            var queueInfo = QueueInfoFactory.Create(queue);
            var queueTask = client.CreateQueueAsync(queue.Vhost, queueInfo);
            _ = tasks.Append(queueTask);
        }

        await Task.WhenAll(tasks);

        tasks = Enumerable.Empty<Task>();
        var errorBindings = Enumerable.Empty<Binding>();

        foreach (var binding in bindings)
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
