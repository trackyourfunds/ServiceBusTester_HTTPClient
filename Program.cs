using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        // Parse command line arguments
        string messageFilePath = "message.json"; // Default value
        
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-message" && i + 1 < args.Length)
            {
                messageFilePath = args[i + 1];
                break;
            }
        }

        // Read and validate message file
        string messageBody;
        Payout payoutItem = null;
        try
        {
            if (!File.Exists(messageFilePath))
            {
                Console.WriteLine($"Error: Message file '{messageFilePath}' not found.");
                return;
            }

            string fileContent = await File.ReadAllTextAsync(messageFilePath);
            
            // Validate JSON format
            try
            {
                JsonDocument.Parse(fileContent);
                
                // Deserialize to payoutItem
                payoutItem = JsonSerializer.Deserialize<Payout>(fileContent);
                
                messageBody = fileContent;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error: '{messageFilePath}' is not a valid JSON format.");
                Console.WriteLine($"Reason: {ex.Message}");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file '{messageFilePath}': {ex.Message}");
            return;
        }

        // Verify cause.name is not null or empty before sending
        if (payoutItem?.cause == null || string.IsNullOrWhiteSpace(payoutItem.cause.name))
        {
            Console.WriteLine("Error: 'cause name' property must not be null or empty.");
            return;
        }

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("ServiceBus_HttpClient.appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("ServiceBus_HttpClient.appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        // Read values from configuration
        string connectionString = configuration["ServiceBus:ConnectionString"] ?? throw new InvalidOperationException("ConnectionString is not configured");
        string queueName = configuration["ServiceBus:QueueName"] ?? throw new InvalidOperationException("TopicName is not configured");

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
        string resourceUri = $"https://{host}/{queueName}";

        // Create SAS (Shared Access Signature) token
        var expiry = DateTimeOffset.UtcNow.AddYears(10).ToUnixTimeSeconds();
        string stringToSign = $"{HttpUtility.UrlEncode(resourceUri)}\n{expiry}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

        string sasToken = $"SharedAccessSignature sr={HttpUtility.UrlEncode(resourceUri)}&sig={HttpUtility.UrlEncode(signature)}&se={expiry}&skn={keyName}";

        Console.WriteLine($"Posting to: {resourceUri}/messages");
        Console.WriteLine($"Authorization: {sasToken}\n");

        // Print token expiration date/time (moved below Authorization message)
        var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expiry).UtcDateTime;
        Console.WriteLine($"Token expires at (UTC): {expiryDateTime:yyyy-MM-dd HH:mm:ss}");

        Console.WriteLine($"Message: {messageBody}");


        // Send message
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", sasToken);

        // Send as JSON
        var content = new StringContent(messageBody, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{resourceUri}/messages", content);

        Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"✓ SUCCESS! JSON message sent to Service Bus Queue {queueName}.");
        }
        else
        {
            Console.WriteLine($"✗ FAILED");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}

