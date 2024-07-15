using EasyNetQ.Management.Client.Model;
using RabbitMQMigrator.Models;
using System.Collections.Generic;

namespace RabbitMQMigrator.Factories;

public static class ComponentModelFactory
{
    public static ComponentModel Create(IEnumerable<Exchange> exchanges, IEnumerable<Queue> queues, IEnumerable<Binding> bindings) => new()
    {
        Exchanges = ExchangesFilter.Filter(exchanges),
        Queues = queues,
        Bindings = bindings,
    };
}
