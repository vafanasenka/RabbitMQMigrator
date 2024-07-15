using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;

namespace RabbitMQMigrator.Models;

public class ComponentModel
{
    public IEnumerable<Exchange> Exchanges { get; set; }

    public IEnumerable<Queue> Queues { get; set; }

    public IEnumerable<Binding> Bindings { get; set; }

    public IEnumerable<Message> Messages { get; set; }
}
