using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;

namespace RabbitMQMigrator;

public static class DataLogger
{
    public static void Log(Dictionary<string, object> settings)
    {
        var exchanges = (IEnumerable<Exchange>)settings[Constants.Settings.ExchangesKey];
        Logger.Log(LogType.Log_Exchanges_Start);
        foreach (var exchange in exchanges)
        {
            LogExchange(exchange);
        }

        var queues = (IEnumerable<Queue>)settings[Constants.Settings.QueuesKey];
        Logger.Log(LogType.Log_Queues_Start);
        foreach (var queue in queues)
        {
            LogQueue(queue);
        }
        
        var bindings = (IEnumerable<Binding>)settings[Constants.Settings.BindingsKey];
        Logger.Log(LogType.Log_Bindings_Start);
        foreach (var binding in bindings)
        {
            LogBinding(binding);
        }
    }

    public static void LogExchange(Exchange exchange)
    {
        Logger.Log(LogType.Log_Exchange_Done, $"Vhost: {exchange.Vhost}; name: {exchange.Name}; type: {exchange.Type}; durable: {exchange.Durable}; AutoDelete: {exchange.AutoDelete}; Internal: {exchange.Internal}");
    }

    public static void LogQueue(Queue queue)
    {
        Logger.Log(LogType.Log_Queue_Done, $"Vhost: {queue.Vhost}; name: {queue.Name}; durable: {queue.Durable}; AutoDelete: {queue.AutoDelete}");
    }

    public static void LogBinding(Binding binding)
    {
        Logger.Log(LogType.Log_Binding_Done, $"Vhost: {binding.Vhost}; source: {binding.Source}; destination: {binding.Destination}; routingKey: {binding.RoutingKey}");
    }
}
