using System;

namespace RabbitMQMigrator;

public static class KeyHandler
{
    public static bool WaitingForYNKeyPress(string message)
    {
        Console.WriteLine($"{message} (Y/N)");
        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Y)
            {
                return true;
            }
            else if (key == ConsoleKey.N)
            {
                return false;
            }
        }
    }
}
