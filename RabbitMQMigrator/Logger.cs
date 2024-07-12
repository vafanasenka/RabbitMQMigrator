using System;
using System.IO;

namespace RabbitMQMigrator;

public static class Logger
{
    private static readonly object _lock = new();
    private static string logFilePath;
    private static readonly string newLine = Environment.NewLine;

    public static string Initialize()
    {
        logFilePath = GetFilePath();

        CreateDirectoryFile(logFilePath);

        return logFilePath;
    }

    public static void Log(LogType logType, string message = "")
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
            throw new ArgumentNullException(nameof(logFilePath));

        lock (_lock)
        {
            message = !string.IsNullOrWhiteSpace(message.Trim()) ? $" - {message.Trim()}" : string.Empty;
            var logMessage = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} - {logType.ToString().ToUpper()}{message}";
            Console.WriteLine(logMessage);
            File.AppendAllText(logFilePath, string.Concat(logMessage, newLine));
        }
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
