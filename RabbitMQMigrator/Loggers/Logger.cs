using System;
using System.IO;
using System.Linq;

namespace RabbitMQMigrator.Loggers;

public static class Logger
{
    private static readonly object _lock = new();
    private static string logFilePath;
    private static readonly string newLine = Environment.NewLine;
    private static readonly LogType[] logTypesRelatedToDoneProcesses = 
        [
            LogType.Get_Settings_Done, LogType.Delete_Settings_Done, LogType.Create_Settings_Done,
            LogType.Delete_Bindings_Done, LogType.Delete_Exchanges_Done, LogType.Delete_Messages_Done, LogType.Delete_Queues_Done,
            LogType.Done,
            LogType.Log_Exchanges_Done, LogType.Log_Bindings_Done, LogType.Log_Queues_Done, LogType.Log_Settings_Done,
            LogType.Migrate_Bindings_Done, LogType.Migrate_Exchanges_Done, LogType.Migrate_Messages_Done, LogType.Migrate_Queues_Done
        ];

    public static string Initialize()
    {
        logFilePath = GetFilePath();

        CreateDirectoryFile(logFilePath);

        return logFilePath;
    }

    public static void Log(LogType logType, string message = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logFilePath, nameof(logFilePath));

        lock (_lock)
        {
            var logMessage = BuildLogLine(logType, message);
            Console.WriteLine(logMessage);
            File.AppendAllText(logFilePath, logMessage);
        }
    }

    private static string BuildLogLine(LogType logType, string message = "")
    {
        bool isLogTypeRelatedToDoneProcesses = logTypesRelatedToDoneProcesses.Contains(logType);

        message = !string.IsNullOrWhiteSpace(message.Trim()) ? $" - {message.Trim()}" : string.Empty;
        var logMessage = string.Concat($"{DateTime.Now:dd-MM-yyyy HH:mm:ss} - {logType.ToString().ToUpper()}{message}", isLogTypeRelatedToDoneProcesses ? string.Concat(newLine, newLine) : newLine);

        return logMessage;
    }

    private static string GetFilePath() => Path.Combine(Constants.Logger.LogFileDisk, Constants.Logger.LogFileDirectory, Constants.Logger.LogFileName);

    private static void CreateDirectoryFile(string filePath)
    {
        var fileDirectory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(fileDirectory))
        {
            Directory.CreateDirectory(fileDirectory);
        }

        if (!File.Exists(filePath))
        {
            var newFile = File.Create(path: filePath);
            newFile.Close();
        }
        else
        {
            File.WriteAllText(filePath, string.Empty);
        }
    }
}
