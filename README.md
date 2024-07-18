RabbitMQMigrator is a .Net console application that provides functionality to migrate key RabbitMQ settings (Exchanges, Qoeues, Bindings) and Messages from 'Source' server to 'Target' server. 
The application uses EasyNetQ.Management.Client library[https://github.com/EasyNetQ/EasyNetQ.Management.Client] as a .NET client for the RabbitMQ Management API. 
Servers settings are set in config.json file. 
The program has a logger which logs the progress both to the console and to the log file along the path: c:\RabbitMQMigratorLogs\migration.log 
