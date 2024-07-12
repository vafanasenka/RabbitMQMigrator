using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQMigrator;

public static class RabbitMQMigrator
{
    public static async Task<Dictionary<string, object>> GetSettings(RabbitMQHttpClient httpClient)
    {
        var queuesTask = httpClient.GetQueuesAsync();

        // TODO exclude exchanges with 'default' user:
        // "user_who_performed_action": "rmq-internal"
        var exchangesTask = httpClient.GetExchangesAsync();

        var bindingsTask = httpClient.GetBindingsAsync();

        await Task.WhenAll(queuesTask, exchangesTask, bindingsTask);

        var settings = new Dictionary<string, object>
        {
            { Constants.Settings.QueuesKey, queuesTask.Result },
            { Constants.Settings.ExchangesKey, exchangesTask.Result },
            { Constants.Settings.BindingsKey, bindingsTask.Result }
        };
        
        return settings;
    }

    public static async Task ApplySettings(RabbitMQHttpClient httpClient, Dictionary<string, object> settings)
    {
        var queues = (JArray)settings[Constants.Settings.QueuesKey];
        var exchanges = (JArray)settings[Constants.Settings.ExchangesKey];
        var bindings = (JArray)settings[Constants.Settings.BindingsKey];

        foreach (var exchange in exchanges)
        {
            string exchangeName = (string)exchange["name"];
            string type = (string)exchange["type"];
            bool durable = (bool)exchange["durable"];
            await httpClient.CreateExchangeAsync(exchangeName, type, durable);
        }

        foreach (var queue in queues)
        {
            string queueName = (string)queue["name"];
            bool durable = (bool)queue["durable"];
            await httpClient.CreateQueueAsync(queueName, durable);
        }

        foreach (var binding in bindings)
        {
            string source = (string)binding["source"];
            string destination = (string)binding["destination"];
            string routingKey = (string)binding["routing_key"];
            await httpClient.CreateBindingAsync(source, destination, routingKey);
        }
    }
}
