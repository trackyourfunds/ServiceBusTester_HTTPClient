# ServiceBus HTTP Client

A simple Windows application for sending JSON messages to Azure Service Bus queues. This tool is designed for testing and validating Service Bus queue configurations without writing code.

## What Does This Tool Do?

ServiceBus HTTP Client allows you to:
- Send test messages to Azure Service Bus queues
- Validate your Service Bus connection and configuration
- Test message processing workflows
- Use custom JSON message templates

## Prerequisites

Before using this tool, you need:
- An Azure Service Bus namespace with at least one queue set up
- A Service Bus connection string (available from the Azure Portal)
- Send permissions on the queue

## Getting Started

### Step 1: Download the Application

1. Go to the [Releases](https://github.com/trackyourfunds/ServiceBusTester_HTTPClient/releases) page
2. Download the latest release ZIP file
3. Extract all files to a folder on your computer

The download includes:
- `ServiceBus_HttpClient.exe` - The application
- `ServiceBus_HttpClient.appsettings.json` - Configuration file
- `message.json` - Sample message template

### Step 2: Configure Your Connection

Open `ServiceBus_HttpClient.appsettings.json` in a text editor (like Notepad) and update the following:

```json
{
  "ServiceBus": {
    "ConnectionString": "YOUR_CONNECTION_STRING_HERE",
    "QueueName": "YOUR_QUEUE_NAME_HERE",
    "TopicName": "",
    "Message": ""
  }
}
```

**Where to find these values:**

- **ConnectionString**: In Azure Portal, go to your Service Bus namespace → Shared access policies → Select a policy → Copy the "Primary Connection String"
- **QueueName**: The name of the queue you want to send messages to

### Step 3: Prepare Your Message

Edit `message.json` with the message content you want to send. This file must contain valid JSON.

**Important:** Your message must include a `cause.name` field. For example:

```json
{
  "amount": 500,
  "payout_id": "po_2025_01_07",
  "cause": {
    "name": "Test_Cause",
    "unit_type": "Non Profit Organization"
  }
}
```

You can customize the rest of the JSON structure based on your needs.

## How to Use

### Sending a Message with the Default Template

1. Open Command Prompt or PowerShell
2. Navigate to the folder where you extracted the files
3. Run the command:
   ```
   ServiceBus_HttpClient.exe
   ```

### Using a Custom Message File

If you want to use a different message file:

```
ServiceBus_HttpClient.exe -message my-custom-message.json
```

## Understanding the Results

When you run the application, you'll see output like this:

**Successful Send:**
```
Posting to: https://your-namespace.servicebus.windows.net/your-queue/messages
Authorization: SharedAccessSignature sr=...

Token expires at (UTC): 2035-01-07 12:34:56
Message: {"amount":500,...}
Status: 201 Created
✓ SUCCESS! JSON message sent to Service Bus Queue your-queue-name.
```

**Failed Send:**
```
Status: 401 Unauthorized
✗ FAILED
...error details...
```

## Troubleshooting

### "Error: Message file 'message.json' not found"
**Solution:** Make sure `message.json` is in the same folder as the `.exe` file, or provide the full path using the `-message` parameter.

### "Error: 'cause name' property must not be null or empty"
**Solution:** Your JSON message must include a `cause` object with a `name` field that is not empty.

### "Status: 401 Unauthorized"
**Possible causes:**
- Your connection string is incorrect
- Your access policy doesn't have "Send" permissions
- The connection string has expired

**Solution:** 
1. Go to Azure Portal
2. Navigate to your Service Bus namespace → Shared access policies
3. Ensure the policy has "Send" permission checked
4. Copy the connection string again and update your `appsettings.json`

### "Status: 404 Not Found"
**Possible causes:**
- The queue name doesn't exist
- The queue name is misspelled

**Solution:** Verify the queue name in Azure Portal matches exactly what's in your `appsettings.json` file (case-sensitive).

### "Error: ... is not a valid JSON format"
**Solution:** Use a JSON validator (like [jsonlint.com](https://jsonlint.com)) to check your message file for syntax errors.

## Tips

- **Test Connection**: Run the tool with the sample `message.json` first to verify your connection works
- **Multiple Messages**: You can create multiple JSON files for different test scenarios
- **Keep Backups**: Save copies of your configuration files before making changes
- **Security**: Keep your connection string secure - don't share it publicly


## Additional Resources

- [Azure Service Bus Documentation](https://docs.microsoft.com/azure/service-bus-messaging/)
- [JSON Validator](https://jsonlint.com) - Check if your JSON is valid
- [GitHub Issues](https://github.com/trackyourfunds/ServiceBusTester_HTTPClient/issues) - Report bugs or request features
