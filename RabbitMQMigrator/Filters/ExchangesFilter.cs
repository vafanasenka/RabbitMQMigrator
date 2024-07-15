using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQMigrator.Filters;

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
        var excludedNames = GetSystemNames();
        return exchanges.Where(_ => !excludedNames.Contains(_.Name));
    }

    private static string[] GetSystemNames() => [string.Empty, "amq.direct", "amq.fanout", "amq.headers", "amq.match", "amq.rabbitmq.trace", "amq.topic"];
}
