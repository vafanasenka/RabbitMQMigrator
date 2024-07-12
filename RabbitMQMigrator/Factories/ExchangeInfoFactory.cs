using EasyNetQ.Management.Client.Model;
using System;

namespace RabbitMQMigrator.Factories;

public static class ExchangeInfoFactory
{
    public static ExchangeInfo Create(Exchange exchange)
    {
        ArgumentNullException.ThrowIfNull(exchange);

        return DoCreate(exchange);
    }

    private static ExchangeInfo DoCreate(Exchange exchange) => new(exchange.Name, exchange.Type)
    {
        Durable = exchange.Durable,
        AutoDelete = exchange.AutoDelete,
        Internal = exchange.Internal,
        Arguments = exchange.Arguments
    };
}
