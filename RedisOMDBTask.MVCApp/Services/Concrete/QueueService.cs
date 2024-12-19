using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using RedisOMDBTask.MVCApp.Services.Abstract;

namespace RedisOMDBTask.MVCApp.Services.Concrete
{
    public class QueueService : IQueueService
    {
        private readonly QueueClient queueClient;

        public QueueService(string connectionString, string queueName)
        {
            queueClient = new QueueClient(connectionString, queueName);
            queueClient.CreateIfNotExists();  // Ensure the queue exists
        }

        // Deletes a message from the queue
        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            await queueClient.DeleteMessageAsync(messageId, popReceipt);
        }

        // Receives a message from the queue
        public async Task<QueueMessage> ReceiveMessageAsync()
        {
            var messageResponse = await queueClient.ReceiveMessageAsync(TimeSpan.FromSeconds(2)); // 2-second visibility timeout
            if (messageResponse.Value != null)
            {
                var message = messageResponse.Value;

                // Display the plain UTF-8 message
                Console.WriteLine($"Received message: {message.MessageText}");

                // Optionally delete the message after processing
                // await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                return message;
            }
            return null;
        }

        // Sends a message to the queue
        public async Task SendMessageAsync(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                try
                {
                    // Send the message as UTF-8 text (no Base64 encoding)
                    var utf8Message = System.Text.Encoding.UTF8.GetBytes(message);

                    // Send the message to the queue with no TTL (message will stay until deleted)
                    await queueClient.SendMessageAsync(Convert.ToBase64String(utf8Message), visibilityTimeout: TimeSpan.Zero);

                    Console.WriteLine("Message sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Message is empty or whitespace.");
            }
        }
    }
}
