using EasyNetQ.Management.Client.Model;
using System;

namespace RabbitMQMigrator.Factories;

public static class QueueInfoFactory
{
    public static QueueInfo Create(Queue queue)
    {
        ArgumentNullException.ThrowIfNull(queue);

        return DoCreate(queue);
    }

    private static QueueInfo DoCreate(Queue queue) => new(queue.Name)
    {
        Durable = queue.Durable,
        AutoDelete = queue.AutoDelete,
        Arguments = queue.Arguments
    };
}
