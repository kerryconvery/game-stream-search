using System;
using System.Threading.Tasks;
using GameStreamSearch.Domain;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Repositories.InMemoryRepositories
{
    public class InMemoryChannelRepository : IChannelRepository
    {
        private Dictionary<string, StreamChannel> channelStore;

        public InMemoryChannelRepository()
        {
            channelStore = new Dictionary<string, StreamChannel>();
        }

        public Task<StreamChannel> GetchannelById(string channelId)
        {
            if (!channelStore.ContainsKey(channelId))
            {
                return Task.FromResult<StreamChannel>(null);
            }

            return Task.FromResult(channelStore[channelId]);
        }

        public Task Savechannel(StreamChannel channel)
        {
            channelStore.Add(channel.ChannelName, channel);

            return Task.FromResult<object>(null);
        }


        public Task SaveChannel(StreamChannel channel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StreamChannel>> GetChannels()
        {
            throw new NotImplementedException();
        }

        public Task<StreamChannel> GetChannelByNameAndPlatform(string channelName, StreamPlatformType streamingPlatform)
        {
            var channel = channelStore.Values.FirstOrDefault(s => s.ChannelName.CompareTo(channelName) == 0 && s.StreamPlatform == streamingPlatform);

            return Task.FromResult(channel);
        }
    }
}
