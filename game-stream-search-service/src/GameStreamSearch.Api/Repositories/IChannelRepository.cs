using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Application.Repositories
{
    public interface IChannelRepository
    {
        Task Add(StreamChannel channel);
        Task<IEnumerable<StreamChannel>> GetAllChannels();
        Task<StreamChannel> GetChannelByNameAndPlatform(string channelName, StreamPlatformType streamingPlatform);
    }
}
