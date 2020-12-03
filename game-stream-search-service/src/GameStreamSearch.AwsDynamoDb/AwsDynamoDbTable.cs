using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using GameStreamSearch.Repositories;

namespace GameStreamSearch.AwsDynamoDb
{
    public class AwsDynamoDbTable<T> : IAwsDynamoDbTable<T>, IDisposable
    {
        private DynamoDBContext dynamoDbContext;
        private AmazonDynamoDBClient dynamoDbClient;

        public AwsDynamoDbTable()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.APSoutheast2,
                
            };

            dynamoDbClient = new AmazonDynamoDBClient(clientConfig);
            dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        }

        public Task PutItem(T item)
        {
            return dynamoDbContext.SaveAsync(item);
        }

        public Task UpdateItem(T item)
        {
            return dynamoDbContext.SaveAsync(item);
        }

        public Task DeleteItem(object partitionKey, object rangeKey)
        {
            return dynamoDbContext.DeleteAsync<T>(partitionKey, rangeKey);
        }

        public Task<T> GetItem(object partitionKey, object rangeKey)
        {
            return dynamoDbContext.LoadAsync<T>(partitionKey, rangeKey);
        }

        public async Task<IEnumerable<T>> GetAllItems(object partitionKey)
        {
            var query = dynamoDbContext.QueryAsync<T>(partitionKey);

            return await query.GetRemainingAsync();
        }

        public void Dispose()
        {
            dynamoDbContext.Dispose();
            dynamoDbClient.Dispose();
        }
    }
}
