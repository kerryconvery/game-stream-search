using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStreamSearch.Repositories
{
    public interface IAwsDynamoDbTable<T>
    {
        Task PutItem(T document);
        Task<T> GetItem(object partitionKey, object rangeKey);
        Task<IEnumerable<T>> GetAllItems();
        Task UpdateItem(T document);
        Task DeleteItem(object partitionKey, object rangeKey);
    }
}
