using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Repositories.InMemoryRepositories
{
    public class InMemoryChannelRepository : IChannelRepository
    {
        private Dictionary<string, Channel> dataStore;

        public InMemoryChannelRepository()
        {
            dataStore = new Dictionary<string, Channel>();
        }

        public Task Add(Channel channel)
        {
            dataStore.Add(channel.StreamPlatform + channel.ChannelName, channel);

            return Task.FromResult<object>(null);
        }

        public Task<Channel> Get(StreamPlatformType streamPlatform, string channelName)
        {
            var key = streamPlatform + channelName;

            if (dataStore.ContainsKey(key))
            {
                return Task.FromResult(dataStore[key]);
            }

            return Task.FromResult<Channel>(null);
        }

        public Task<ChannelListDto> SelectAllChannels()
        {
            var channels = new ChannelListDto
            {
                Items = dataStore.Values.Select(c => new ChannelDto {
                    ChannelName = c.ChannelName,
                    StreamPlatform = c.StreamPlatform,
                    AvatarUrl = c.AvatarUrl,
                    ChannelUrl = c.ChannelUrl,
                })
            };

            return Task.FromResult(channels);
        }

        public Task Remove(StreamPlatformType streamPlatform, string channelName)
        {
            var key = streamPlatform + channelName;

            if (dataStore.ContainsKey(key))
            {
                dataStore.Remove(key);
            }

            return Task.FromResult<object>(null);
        }
    }
}
