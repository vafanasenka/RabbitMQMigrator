namespace RabbitMQMigrator;

public class ServerModel
{
    public string HostName { get; set; }

    public int ManagementPort { get; set; }

    public int AMQPPort { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}
