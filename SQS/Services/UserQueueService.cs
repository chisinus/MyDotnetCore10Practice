using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using SQS.Models;

namespace SQS.Services
{
    public class UserQueueService : IUserQueueService
    {
        private readonly AmazonSQSClient _sqs;
        private readonly string _queueUrl;

        public UserQueueService(bool useLocal = true)
        {
            var config = new AmazonSQSConfig();

            if (useLocal)
                config.ServiceURL = "http://localhost:4566";

            _sqs = new AmazonSQSClient(config);
            _queueUrl = "http://localhost:4566/000000000000/xj-queue";
        }

        public async Task<bool> CreateQueueAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _sqs.CreateQueueAsync(_queueUrl, cancellationToken);
                return true;
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine($"Error creating queue: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendMessageAsync(SQSMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                await _sqs.SendMessageAsync(_queueUrl, json, cancellationToken);
                return true;
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> ReceiveMessagesAsync(int maxNumberOfMessages = 10, int waitTimeSeconds = 20, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = maxNumberOfMessages,
                    WaitTimeSeconds = waitTimeSeconds
                }, cancellationToken);

                return response.Messages.Select(m => m.Body).ToList();
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine($"Error receiving messages: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<bool> DeleteMessageAsync(string messageId, string receiptHandle, CancellationToken cancellationToken = default)
        {
            try
            {
                await _sqs.DeleteMessageAsync(_queueUrl, receiptHandle, cancellationToken);
                return true;
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine($"Error deleting message: {ex.Message}");
                return false;
            }
        }
    }
}
