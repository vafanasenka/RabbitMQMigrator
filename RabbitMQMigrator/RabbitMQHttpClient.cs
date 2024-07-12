using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQMigrator;

public class RabbitMQHttpClient : IDisposable
{
    private readonly HttpClient httpClient;

    public RabbitMQHttpClient(string url, string userName, string password)
    {
        httpClient = new HttpClient { BaseAddress = new Uri(url) };
        var authToken = Encoding.ASCII.GetBytes($"{userName}:{password}");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
    }

    public async Task<JArray> GetQueuesAsync()
    {
        var response = await httpClient.GetStringAsync("api/queues");
        return JArray.Parse(response);
    }

    public async Task<JArray> GetExchangesAsync()
    {
        var response = await httpClient.GetStringAsync("api/exchanges");
        return JArray.Parse(response);
    }

    public async Task<JArray> GetBindingsAsync()
    {
        var response = await httpClient.GetStringAsync("api/bindings");
        return JArray.Parse(response);
    }

    public async Task CreateQueueAsync(string queueName, bool durable)
    {
        var content = new StringContent($"{{\"durable\": {durable.ToString().ToLower()}}}", Encoding.UTF8, "application/json");
        await httpClient.PutAsync($"api/queues/%2F/{queueName}", content);
    }

    public async Task CreateExchangeAsync(string exchangeName, string type, bool durable)
    {
        var content = new StringContent($"{{\"type\": \"{type}\", \"durable\": {durable.ToString().ToLower()}}}", Encoding.UTF8, "application/json");
        await httpClient.PutAsync($"api/exchanges/%2F/{exchangeName}", content);
    }

    public async Task CreateBindingAsync(string source, string destination, string routingKey)
    {
        var content = new StringContent($"{{\"routing_key\": \"{routingKey}\"}}", Encoding.UTF8, "application/json");
        await httpClient.PostAsync($"api/bindings/%2F/e/{source}/q/{destination}", content);
    }

    public void Dispose() => httpClient.Dispose();
}
