using EasyNetQ.Management.Client.Model;
using RabbitMQMigrator.Models;

namespace RabbitMQMigrator;

public static class DataLogger
{
    public static void Log(SettingModel settings)
    {
        Logger.Log(LogType.Log_Settings_Start, "Log Settings from Source server...");
        var counter = 0;
        Logger.Log(LogType.Log_Exchanges_Start);

        foreach (var exchange in settings.Exchanges)
        {
            LogExchange(exchange);
            counter += 1;
        }

        Logger.Log(LogType.Log_Exchanges_Done, $"Exchages total count: {counter}");
        counter = 0;

        Logger.Log(LogType.Log_Queues_Start);

        foreach (var queue in settings.Queues)
        {
            LogQueue(queue);
            counter += 1;
        }

        Logger.Log(LogType.Log_Queues_Done, $"Queues total count: {counter}");
        counter = 0;

        Logger.Log(LogType.Log_Bindings_Start);

        foreach (var binding in settings.Bindings)
        {
            LogBinding(binding);
            counter += 1;
        }

        Logger.Log(LogType.Log_Bindings_Done, $"Bindings total count: {counter}");
        Logger.Log(LogType.Log_Settings_Done);
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
