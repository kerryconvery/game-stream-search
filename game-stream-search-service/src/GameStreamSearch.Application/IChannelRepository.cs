using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public interface IChannelRepository
    {
        Task Add(Channel channel);
        Task<Maybe<Channel>> Get(string streamPlatformId, string channelName);
        Task Update(Channel channel);
        Task Remove(string streamPlatformId, string channelName);
        Task<ChannelListDto> SelectAllChannels();
    }
}
