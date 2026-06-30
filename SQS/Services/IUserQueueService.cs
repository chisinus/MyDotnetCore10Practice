using SQS.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SQS.Services
{
    /// <summary>
    /// Defines the contract for interacting with Amazon SQS queue to manage user messages.
    /// </summary>
    public interface IUserQueueService
    {
        /// <summary>
        /// Creates an SQS queue for user messages.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task<bool> CreateQueueAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a user message to the SQS queue.
        /// </summary>
        /// <param name="message">The SQSMessage to send.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task<bool> SendMessageAsync(SQSMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Receives user messages from the SQS queue.
        /// </summary>
        /// <param name="maxNumberOfMessages">The maximum number of messages to receive.</param>
        /// <param name="waitTimeSeconds">The time to wait for messages.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of user message strings.</returns>
        Task<List<string>> ReceiveMessagesAsync(int maxNumberOfMessages = 10, int waitTimeSeconds = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a user message from the SQS queue.
        /// </summary>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <param name="receiptHandle">The receipt handle of the message to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task<bool> DeleteMessageAsync(string messageId, string receiptHandle, CancellationToken cancellationToken = default);
    }
}