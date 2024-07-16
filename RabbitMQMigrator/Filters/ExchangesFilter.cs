using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQMigrator.Filters;

public static class ExchangesFilter
{
    public static IEnumerable<Exchange> Filter(IEnumerable<Exchange> exchanges, IEnumerable<string> excludedNames)
    {
        if (exchanges == null || !exchanges.Any() || excludedNames == null || !excludedNames.Any())
            return exchanges;

        return DoFilter(exchanges, excludedNames);
    }

    private static IEnumerable<Exchange> DoFilter(IEnumerable<Exchange> exchanges, IEnumerable<string> excludedNames) => exchanges.Where(_ => !excludedNames.Contains(_.Name));
}
