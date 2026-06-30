using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Services
{
    /// <summary>
    /// Defines the contract for interacting with Amazon DynamoDB database.
    /// </summary>
    public interface IDynamoDBService
    {
        /// <summary>
        /// Adds an item to a DynamoDB table.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="item">The item to add.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task AddAsync<T>(T item, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Updates an existing item in a DynamoDB table.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="item">The item containing updated fields.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task UpdateAsync<T>(T item, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Deletes an item from a DynamoDB table.
        /// </summary>
        /// <param name="hashKey">The partition key of the item to delete.</param>
        /// <param name="rangeKey">The optional range key of the item to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task DeleteAsync(object hashKey, object? rangeKey = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches for items in a DynamoDB table matching criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to return.</typeparam>
        /// <param name="attributeName">The attribute name to query/scan on.</param>
        /// <param name="attributeValue">The value of the attribute to match.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of matching items.</returns>
        Task<IEnumerable<T>> SearchAsync<T>(string attributeName, object attributeValue, CancellationToken cancellationToken = default) where T : class;
    }
}

