using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public interface IChannelRepository
    {
        Task Add(Channel channel);
        Task<Maybe<Channel>> Get(string streamPlatformName, string channelName);
        Task Update(Channel channel);
        Task Remove(string streamPlatformName, string channelName);
        Task<IEnumerable<Channel>> GetAll();
    }
}
