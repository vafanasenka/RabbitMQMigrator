using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQMigrator;

public static class ExchangesFilter
{
    public static IEnumerable<Exchange> Filter(IEnumerable<Exchange> exchanges)
    {
        if (exchanges == null || !exchanges.Any())
            return exchanges;

        return DoFilter(exchanges);
    }

    private static IEnumerable<Exchange> DoFilter(IEnumerable<Exchange> exchanges)
    {
        var excludedNames = GetExcludedNames();
        return exchanges.Where(_ => !excludedNames.Contains(_.Name));
    }

    private static string[] GetExcludedNames() => [string.Empty, "amq.direct", "amq.fanout", "amq.headers", "amq.match", "amq.rabbitmq.trace", "amq.topic"];
}
