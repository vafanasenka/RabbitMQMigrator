using EasyNetQ.Management.Client.Model;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQMigrator.Filters;

public static class BindingsFilter
{
    public static IEnumerable<Binding> Filter(IEnumerable<Binding> bindings, IEnumerable<string> excludedNames)
    {
        if (bindings == null || !bindings.Any() || excludedNames == null || !excludedNames.Any())
            return bindings;

        return DoFilter(bindings, excludedNames);
    }

    private static IEnumerable<Binding> DoFilter(IEnumerable<Binding> bindings, IEnumerable<string> excludedNames)
    {
        return bindings.Where(_ => !excludedNames.Contains(_.Source) && !excludedNames.Contains(_.Destination));
    }
}
