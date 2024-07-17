using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace RabbitMQMigrator.Obsolete;

[Obsolete("It not tested, just as an idea")]
public class MessageMigrator
{
    public static void MigrateMessages(IConnection sourceConnection, IConnection targetConnection, string queueName)
    {
        using var sourceChannel = sourceConnection.CreateModel();
        using var targetChannel = targetConnection.CreateModel();

        var consumer = new EventingBasicConsumer(sourceChannel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var properties = targetChannel.CreateBasicProperties();
            properties.Persistent = true;

            targetChannel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
            sourceChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        sourceChannel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

        Console.WriteLine($"Migrating messages from queue {queueName}...");
        // TODO: review this sleep and redevelop it?
        System.Threading.Thread.Sleep(10000);
    }
}
