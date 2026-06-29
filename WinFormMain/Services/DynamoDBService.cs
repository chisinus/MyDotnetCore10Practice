using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace WinFormMain.Services;

/// <summary>
/// Implementation of <see cref="IDynamoDBService"/> providing DynamoDB operations against a local instance.
/// </summary>
public class DynamoDBService : IDynamoDBService, IDisposable
{
    private readonly AmazonDynamoDBClient _client;
    private readonly DynamoDBContext _context;

    public DynamoDBService()
    {
        var clientConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000",
            //RegionEndpoint = Amazon.RegionEndpoint.USEast1
        };

        // Use dummy credentials for local DynamoDB
        //var credentials = new Amazon.Runtime.BasicAWSCredentials("XJAccessKeyID", "XJSecretAccessKey");
        //_client = new AmazonDynamoDBClient(credentials, clientConfig);
        _client = new AmazonDynamoDBClient(clientConfig);
#pragma warning disable CS0618 // Type or member is obsolete
        _context = new DynamoDBContext(_client);
#pragma warning restore CS0618
    }

    private async Task EnsureTableExistsAsync(string tableName, CancellationToken cancellationToken)
    {
        try
        {
            var tableResponse = await _client.ListTablesAsync(cancellationToken);
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
                await _client.CreateTableAsync(createRequest, cancellationToken);

                // Wait until the table is active
                bool isTableActive = false;
                while (!isTableActive)
                {
                    await Task.Delay(500, cancellationToken);
                    var describeResponse = await _client.DescribeTableAsync(tableName, cancellationToken);
                    isTableActive = describeResponse.Table.TableStatus == TableStatus.ACTIVE;
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to ensure table '{tableName}' exists.", ex);
        }
    }

    /// <inheritdoc />
    public async Task AddAsync<T>(string tableName, T item, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(item);

        await EnsureTableExistsAsync(tableName, cancellationToken);

        var config = new SaveConfig { OverrideTableName = tableName };
        await _context.SaveAsync(item, config, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync<T>(string tableName, T item, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(item);

        await EnsureTableExistsAsync(tableName, cancellationToken);

        var config = new SaveConfig { OverrideTableName = tableName };
        await _context.SaveAsync(item, config, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string tableName, object hashKey, object? rangeKey = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(hashKey);

        await EnsureTableExistsAsync(tableName, cancellationToken);

        var keyMap = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = hashKey.ToString() } }
        };
        var deleteRequest = new DeleteItemRequest
        {
            TableName = tableName,
            Key = keyMap
        };
        await _client.DeleteItemAsync(deleteRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> SearchAsync<T>(string tableName, string attributeName, object attributeValue, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        await EnsureTableExistsAsync(tableName, cancellationToken);

        var config = new ScanConfig { OverrideTableName = tableName };

        // If attributeName is empty, return all items (scan)
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            var search = _context.ScanAsync<T>(new List<ScanCondition>(), config);
            var results = await search.GetRemainingAsync(cancellationToken);
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
            var results = await search.GetRemainingAsync(cancellationToken);
            return results;
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
        _context?.Dispose();
    }
}
