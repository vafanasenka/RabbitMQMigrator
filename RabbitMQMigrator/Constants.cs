namespace RabbitMQMigrator;

public class Constants
{
    public class Config
    {
        public const string ConfigFileName = "config.json";

        public const string SourceServer = "SourceServer";

        public const string TargetServer = "TargetServer";
    }

    public class Logger
    {
        public const string LogFileName = "migration.log";

        public const string LogFileDirectory = "RabbitMQMigratorLogs";

        public const string LogFileDisk = "C:";
    }

    public class Server
    {
        public const string HostName = "HostName";

        public const string ManagementPort = "ManagementPort";

        public const string AMQPPort = "AMQPPort";

        public const string UserName = "UserName";

        public const string Password = "Password";
    }
}
