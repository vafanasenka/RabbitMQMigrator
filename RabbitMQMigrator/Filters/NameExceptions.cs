using System.Collections.Generic;
using System.Linq;

namespace RabbitMQMigrator.Filters;

public class NameExceptions
{
    private readonly IEnumerable<string> nameExceptions = ["celery.pidbox", string.Empty, "amq.direct", "amq.fanout", "amq.headers", "amq.match", "amq.rabbitmq.trace", "amq.topic"];

    public IEnumerable<string> GetForExchanges() => nameExceptions;

    public IEnumerable<string> GetForQueues() => nameExceptions.Take(1);

    public IEnumerable<string> GetForBindings() => nameExceptions.Take(2);
}
