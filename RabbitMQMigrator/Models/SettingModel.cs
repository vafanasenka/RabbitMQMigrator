using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;

namespace RabbitMQMigrator.Models;

public class SettingModel
{
    public IEnumerable<Exchange> Exchanges { get; set; }

    public IEnumerable<Queue> Queues { get; set; }

    public IEnumerable<Binding> Bindings { get; set; }
}
