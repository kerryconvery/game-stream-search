using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public interface IRepository<T>
    {
        Task Add(T channel);
        Task<Maybe<T>> Get(string streamPlatformName, string channelName);
        Task Update(T channel);
        Task Remove(string streamPlatformName, string channelName);
        Task<IEnumerable<T>> GetAll();
    }
}
