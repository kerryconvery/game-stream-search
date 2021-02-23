using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public interface IRepository<T>
    {
        Task Add(T channel);
        Task<Maybe<T>> Get(string streamPlatformId, string channelName);
        Task Update(T channel);
        Task Remove(string streamPlatformId, string channelName);
        Task<IEnumerable<T>> GetAll();
    }
}
