using EasyNetQ.Management.Client.Model;
using RabbitMQ.Client;
using RabbitMQMigrator.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQMigrator.Migrators;

public class MessageMigrator
{
    private readonly IConnection sourceConnection;
    private readonly IConnection targetConnection;
    private readonly IEnumerable<Queue> queues;

    public MessageMigrator(IConnection sourceConnection, IConnection targetConnection, IEnumerable<Queue> queues)
    {
        ArgumentNullException.ThrowIfNull(sourceConnection, nameof(sourceConnection));
        ArgumentNullException.ThrowIfNull(targetConnection, nameof(targetConnection));
        ArgumentNullException.ThrowIfNull(queues, nameof(queues));

        this.sourceConnection = sourceConnection;
        this.targetConnection = targetConnection;
        this.queues = queues;
    }

    public async Task Migrate()
    {
        using var sourceChannel = sourceConnection.CreateModel();
        using var targetChannel = targetConnection.CreateModel();

        var counter = 0;
        Logger.Log(LogType.Migrate_Messages_Start);

        foreach (var queue in queues)
        {
            // we need to migrate only unacknowledged messages?

            var queueName = queue.Name;
            var result = sourceChannel.BasicGet(queueName, autoAck: false);
            Logger.Log(LogType.Migrate_Message, $"from Queue: {queueName}");

            while (result != null)
            {
                var body = result.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (IsUnhandledMessage(result))
                {
                    await Task.Run(() =>
                    {
                        targetChannel.BasicPublish(exchange: "",
                                                   routingKey: queueName,
                                                   basicProperties: null,
                                                   body: body);


                        // do we need to mark as Acknowledged all messages in Source server?
                        //sourceChannel.BasicAck(deliveryTag: result.DeliveryTag, multiple: false);

                        counter += 1;
                        Logger.Log(LogType.Migrate_Message, $"Message {counter} : {message}");
                    });
                }

                result = sourceChannel.BasicGet(queueName, autoAck: false);
            }
        }

        Logger.Log(LogType.Migrate_Messages_Done);
    }

    private static bool IsUnhandledMessage(BasicGetResult result)
    {
        // Implement logic to determine if a message is 'unhandled'
        // Check a specific header?
        if (result.BasicProperties.Headers != null &&
            result.BasicProperties.Headers.TryGetValue("x-handled", out var handledValue) &&
            handledValue is byte[] handledBytes &&
            Encoding.UTF8.GetString(handledBytes) == "true")
        {
            return false;
        }

        return true;
    }
}
