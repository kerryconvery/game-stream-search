using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace GameStreamSearch.Repositories
{
    public class AwsDynamoDbGateway<T> : IDisposable
    {
        private DynamoDBContext dynamoDbContext;
        private AmazonDynamoDBClient dynamoDbClient;

        public AwsDynamoDbGateway()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
            {
                //RegionEndpoint = RegionEndpoint.APSoutheast2,
                ServiceURL = "http://localhost:8000"
            };

            dynamoDbClient = new AmazonDynamoDBClient(clientConfig);
            dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        }

        public Task PutItem(T item)
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

        public async Task<IEnumerable<T>> GetAllItems()
        {
            var scan = dynamoDbContext.ScanAsync<T>(null);

            return await scan.GetRemainingAsync();
        }

        public void Dispose()
        {
            dynamoDbContext.Dispose();
            dynamoDbClient.Dispose();
        }
    }
}
