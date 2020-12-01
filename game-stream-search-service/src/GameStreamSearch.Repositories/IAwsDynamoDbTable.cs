using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStreamSearch.Repositories
{
    public interface IAwsDynamoDbTable<T>
    {
        Task PutItem(T document);
        Task<T> GetItem(string primaryKey, string sortKey);
        Task<IEnumerable<T>> GetAllItems();
        Task UpdateItem(T document);
        Task DeleteItem(string primaryKey, string sortKey);
    }
}
