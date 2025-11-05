using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("ServiceBus_HttpClient.appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("ServiceBus_HttpClient.appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        // Read values from configuration
        string connectionString = configuration["ServiceBus:ConnectionString"] ?? throw new InvalidOperationException("ConnectionString is not configured");
        string topicName = configuration["ServiceBus:TopicName"] ?? throw new InvalidOperationException("TopicName is not configured");
        string messageBody = configuration["ServiceBus:Message"] ?? throw new InvalidOperationException("Message is not configured");

        // Parse connection string
        var parts = connectionString.Split(';');
        string endpoint = null, keyName = null, key = null;
        foreach (var part in parts)
        {
            if (part.StartsWith("Endpoint=")) endpoint = part.Substring(9);
            if (part.StartsWith("SharedAccessKeyName=")) keyName = part.Substring(20);
            if (part.StartsWith("SharedAccessKey=")) key = part.Substring(16);
        }

        string host = endpoint.Replace("sb://", "").TrimEnd('/');
        string resourceUri = $"https://{host}/{topicName}";

        // Create SAS (Shared Access Signature) token
        var expiry = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        string stringToSign = $"{HttpUtility.UrlEncode(resourceUri)}\n{expiry}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

        string sasToken = $"SharedAccessSignature sr={HttpUtility.UrlEncode(resourceUri)}&sig={HttpUtility.UrlEncode(signature)}&se={expiry}&skn={keyName}";

        Console.WriteLine($"Posting to: {resourceUri}/messages");
        Console.WriteLine($"Message: {messageBody}");
        Console.WriteLine($"Authorization: {sasToken}\n");

        // Send message
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", sasToken);

        // Send as JSON
        var content = new StringContent(messageBody, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{resourceUri}/messages", content);

        Console.WriteLine($"Status: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("✓ SUCCESS! JSON message sent to Service Bus topic.");
        }
        else
        {
            Console.WriteLine($"✗ FAILED");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}

