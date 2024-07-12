using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace RabbitMQMigrator;

public class ConfigurationProvider
{
    public static JToken GetConfiguration(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentNullException(nameof(type));

        return GetConfiguration()[type];
    }

    private static JObject GetConfiguration()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var configFilePath = Path.Combine(baseDirectory, "..", "..", "..", Constants.Config.ConfigFileName);
        configFilePath = Path.GetFullPath(configFilePath);
        return JObject.Parse(File.ReadAllText(configFilePath));
    }
}
