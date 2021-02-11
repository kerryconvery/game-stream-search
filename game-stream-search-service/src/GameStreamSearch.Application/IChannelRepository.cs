using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public interface IChannelRepository
    {
        Task Add(Channel channel);
        Task<Maybe<Channel>> Get(StreamPlatformType streamPlatform, string channelName);
        Task Update(Channel channel);
        Task Remove(StreamPlatformType streamPlatform, string channelName);
        Task<ChannelListDto> SelectAllChannels();
    }
}
