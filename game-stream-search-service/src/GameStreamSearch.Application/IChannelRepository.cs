using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public interface IChannelRepository
    {
        Task Add(Channel channel);
        Task<Channel?> Get(StreamPlatformType streamPlatform, string channelName);
        Task Update(Channel channel);
        Task Remove(StreamPlatformType streamPlatform, string channelName);
        Task<ChannelListDto> SelectAllChannels();
    }
}
