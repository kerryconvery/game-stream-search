using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GameStreamSearch.Repositories;

namespace GameStreamSearch.AwsDynamoDb
{
    public class AwsDynamoDbTable<T> : IAwsDynamoDbTable<T>
    {
        private DynamoDBContext dynamoDbContext;
        private AmazonDynamoDBClient dynamoDbClient;

        public AwsDynamoDbTable()
        {
            dynamoDbClient = new AmazonDynamoDBClient();

            dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        }

        public Task PutItem(T item)
        {
            return dynamoDbContext.SaveAsync(item);
        }

        public Task<T> GetItem(string primaryKey, string sortKey)
        {
            return dynamoDbContext.LoadAsync<T>(primaryKey, sortKey);
        }

        public async Task<IEnumerable<T>> GetAllItems()
        {
            var batchGet = dynamoDbContext.CreateBatchGet<T>();

            await batchGet.ExecuteAsync();

            return batchGet.Results;
        }

        public Task UpdateItem(T item)
        {
            return dynamoDbContext.SaveAsync(item);
        }

        public Task DeleteItem(string primaryKey, string sortKey)
        {
            return dynamoDbContext.DeleteAsync<T>(primaryKey, sortKey);
        }
    }
}
