using EasyNetQ.Management.Client.Model;
using RabbitMQMigrator.Filters;
using RabbitMQMigrator.Models;
using System.Collections.Generic;

namespace RabbitMQMigrator.Factories;

public static class SettingModelFactory
{
    public static SettingModel Create(IEnumerable<Exchange> exchanges, IEnumerable<Queue> queues, IEnumerable<Binding> bindings)
    {
        var nameExceptions = new NameExceptions();
        return new()
        {
            Exchanges = ExchangesFilter.Filter(exchanges, nameExceptions.GetForExchanges()),
            Queues = QueuesFilter.Filter(queues, nameExceptions.GetForQueues()),
            Bindings = BindingsFilter.Filter(bindings, nameExceptions.GetForBindings())
        };
    }
}
