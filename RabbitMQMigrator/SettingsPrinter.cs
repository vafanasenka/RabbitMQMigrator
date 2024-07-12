using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RabbitMQMigrator;

public class SettingsPrinter
{
    public static void Print(Dictionary<string, object> settings)
    {
        var exchanges = (JArray)settings[Constants.Settings.ExchangesKey];
        Logger.Log(LogType.Print_Exchanges_Start);
        foreach (var exchange in exchanges)
        {
            PrintExchange(exchange);
        }

        var queues = (JArray)settings[Constants.Settings.QueuesKey];
        Logger.Log(LogType.Print_Queues_Start);
        foreach (var queue in queues)
        {
            PrintQueue(queue);
        }
        
        var bindings = (JArray)settings[Constants.Settings.BindingsKey];
        Logger.Log(LogType.Print_Bindings_Start);
        foreach (var binding in bindings)
        {
            PrintBinding(binding);
        }
    }

    public static void PrintExchange(JToken exchange)
    {
        var name = (string)exchange["name"];
        var type = (string)exchange["type"];
        var durable = (bool)exchange["durable"];
        
        Logger.Log(LogType.Print_Exchange_Done, $"name: {name}; type: {type}; durable: {durable}");
    }

    public static void PrintQueue(JToken queue)
    {
        var name = (string)queue["name"];
        var durable = (bool)queue["durable"];

        Logger.Log(LogType.Print_Queue_Done, $"name: {name}; durable: {durable}");
    }

    public static void PrintBinding(JToken binding)
    {
        var source = (string)binding["source"];
        var destination = (string)binding["destination"];
        var routingKey = (string)binding["routing_key"];

        Logger.Log(LogType.Print_Binding_Done, $"source: {source}; destination: {destination}; routingKey: {routingKey}");
    }
}
