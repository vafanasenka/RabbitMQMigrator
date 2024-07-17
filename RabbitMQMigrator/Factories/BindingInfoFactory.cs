using EasyNetQ.Management.Client.Model;
using System;

namespace RabbitMQMigrator.Factories;

public static class BindingInfoFactory
{
    public static BindingInfo Create(Binding binding)
    {
        ArgumentNullException.ThrowIfNull(binding, nameof(binding));

        return DoCreate(binding);
    }

    private static BindingInfo DoCreate(Binding binding) => new(binding.RoutingKey)
    {
        Arguments = binding.Arguments
    };
}
