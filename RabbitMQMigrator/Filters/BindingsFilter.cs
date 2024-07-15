using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQMigrator.Filters;

public static class BindingsFilter
{
    public static IEnumerable<Binding> Filter(IEnumerable<Binding> bindings)
    {
        if (bindings == null || !bindings.Any())
            return bindings;

        return DoFilter(bindings);
    }

    private static IEnumerable<Binding> DoFilter(IEnumerable<Binding> bindings)
        => bindings.Where(_ => !string.IsNullOrWhiteSpace(_.Source) && !string.IsNullOrWhiteSpace(_.Destination));
}
