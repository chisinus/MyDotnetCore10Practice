using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DAL.Services;

namespace DAL.Services
{
    /// <summary>
    /// Implementation of <see cref="IDynamoDBService"/> providing DynamoDB operations against a local instance.
    /// </summary>
    public class DynamoDBService : IDynamoDBService, IDisposable
    {
        private const string tableName = "tblUsers";
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;
        private readonly ConcurrentDictionary<string, bool> _verifiedTables = new ConcurrentDictionary<string, bool>();

        public DynamoDBService()
        {
            var localstackHost = Environment.GetEnvironmentVariable("LOCALSTACK_HOSTNAME");
            var endpointUrl = string.IsNullOrEmpty(localstackHost)
                ? "http://localhost:4566"
                : $"http://{localstackHost}:4566";

            var clientConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = endpointUrl            // Local AWS DynamoDB endpoint for LocalStack
            };

            _client = new AmazonDynamoDBClient(clientConfig);
#pragma warning disable CS0618 // Type or member is obsolete
            _context = new DynamoDBContext(_client);
#pragma warning restore CS0618
        }

        private async Task EnsureTableExistsAsync(CancellationToken cancellationToken)
        {
            if (_verifiedTables.ContainsKey(tableName))
            {
                return;
            }

            try
            {
                var tableResponse = await _client.ListTablesAsync(cancellationToken).ConfigureAwait(false);

                if (!tableResponse.TableNames.Contains(tableName))
                {
                    var createRequest = new CreateTableRequest
                    {
                        TableName = tableName,
                        AttributeDefinitions = new List<AttributeDefinition>
                        {
                            new AttributeDefinition
                            {
                                AttributeName = "Id",
                                AttributeType = ScalarAttributeType.S
                            }
                        },
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = "Id",
                                KeyType = KeyType.HASH
                            }
                        },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 5,
                            WriteCapacityUnits = 5
                        }
                    };
                    await _client.CreateTableAsync(createRequest, cancellationToken).ConfigureAwait(false);

                    // Wait until the table is active
                    bool isTableActive = false;
                    while (!isTableActive)
                    {
                        await Task.Delay(500, cancellationToken).ConfigureAwait(false);
                        var describeResponse = await _client.DescribeTableAsync(tableName, cancellationToken).ConfigureAwait(false);
                        isTableActive = describeResponse.Table.TableStatus == TableStatus.ACTIVE;
                    }
                }

                _verifiedTables.TryAdd(tableName, true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to ensure table '{tableName}' exists.", ex);
            }
        }

        /// <inheritdoc />
        public async Task AddAsync<T>(T item, CancellationToken cancellationToken = default) where T : class
        {
            await EnsureTableExistsAsync(cancellationToken).ConfigureAwait(false);

            var config = new SaveConfig { OverrideTableName = tableName };
            await _context.SaveAsync(item, config, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task UpdateAsync<T>(T item, CancellationToken cancellationToken = default) where T : class
        {
            await EnsureTableExistsAsync(cancellationToken).ConfigureAwait(false);

            var config = new SaveConfig { OverrideTableName = tableName };
            await _context.SaveAsync(item, config, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(object hashKey, object? rangeKey = null, CancellationToken cancellationToken = default)
        {
            await EnsureTableExistsAsync(cancellationToken).ConfigureAwait(false);

            var keyMap = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = hashKey.ToString() } }
            };
            var deleteRequest = new DeleteItemRequest
            {
                TableName = tableName,
                Key = keyMap
            };
            await _client.DeleteItemAsync(deleteRequest, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> SearchAsync<T>(string attributeName, object attributeValue, CancellationToken cancellationToken = default) where T : class
        {
            await EnsureTableExistsAsync(cancellationToken).ConfigureAwait(false);

            var config = new ScanConfig { OverrideTableName = tableName };

            // If attributeName is empty, return all items (scan)
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                var search = _context.ScanAsync<T>(new List<ScanCondition>(), config);
                var results = await search.GetRemainingAsync(cancellationToken).ConfigureAwait(false);
                return results;
            }
            else
            {
                // Scan with a filter
                var scanConditions = new List<ScanCondition>
                {
                    new ScanCondition(attributeName, ScanOperator.Contains, attributeValue)
                };
                var search = _context.ScanAsync<T>(scanConditions, config);
                var results = await search.GetRemainingAsync(cancellationToken).ConfigureAwait(false);
                return results;
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            _context?.Dispose();
        }
    }
}

