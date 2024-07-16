using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQMigrator.Filters;

public static class QueuesFilter
{
    public static IEnumerable<Queue> Filter(IEnumerable<Queue> queues, IEnumerable<string> excludedNames)
    {
        if (queues == null || !queues.Any() || excludedNames == null || !excludedNames.Any())
            return queues;

        return DoFilter(queues, excludedNames);
    }

    private static IEnumerable<Queue> DoFilter(IEnumerable<Queue> queues, IEnumerable<string> excludedNames) => queues.Where(_ => excludedNames.Any(name => !_.Name.EndsWith(name)));
}
