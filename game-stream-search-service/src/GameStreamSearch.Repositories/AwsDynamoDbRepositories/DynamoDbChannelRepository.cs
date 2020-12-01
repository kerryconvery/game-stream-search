using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Repositories;
using GameStreamSearch.Repositories.AwsDynamoDbRepositories.Dto;

namespace GameStreamSearch.Repositories.AwsDynamoDbRepositories
{
    public class DynamoDbChannelRepository : IChannelRepository
    {
        private readonly IAwsDynamoDbTable<DynamoDbChannelDto> awsDynamoDbTable;

        public DynamoDbChannelRepository(IAwsDynamoDbTable<DynamoDbChannelDto> awsDynamoDbTable)
        {
            this.awsDynamoDbTable = awsDynamoDbTable;
        }

        public Task Add(Channel channel)
        {
            DynamoDbChannelDto channelDto = new DynamoDbChannelDto
            {
                ChannelName = channel.ChannelName,
                StreamPlatform = channel.StreamPlatform,
                DateRegistered = channel.DateRegistered,
                AvatarUrl = channel.AvatarUrl,
                ChannelUrl = channel.ChannelUrl,
            };

            return awsDynamoDbTable.PutItem(channelDto);
        }

        public async Task<Channel> Get(StreamPlatformType streamPlatform, string channelName)
        {
            var channelDto = await awsDynamoDbTable.GetItem(channelName, streamPlatform.ToString());

            return new Channel
            {
                ChannelName = channelDto.ChannelName,
                StreamPlatform = channelDto.StreamPlatform,
                DateRegistered = channelDto.DateRegistered,
                AvatarUrl = channelDto.AvatarUrl,
                ChannelUrl = channelDto.ChannelUrl,
            };
        }

        public Task Remove(StreamPlatformType streamPlatform, string channelName)
        {
            return awsDynamoDbTable.DeleteItem(channelName, streamPlatform.ToString());
        }

        public async Task<ChannelListDto> SelectAllChannels()
        {
            var channels = await awsDynamoDbTable.GetAllItems();

            ChannelListDto channelList = new ChannelListDto();

            var channelDtos = channels.Select(c => new ChannelDto
            {
                ChannelName = c.ChannelName,
                StreamPlatform = c.StreamPlatform,
                StreamPlatformDisplayName = c.StreamPlatform.GetFriendlyName(),
                AvatarUrl = c.AvatarUrl,
                ChannelUrl = c.ChannelUrl,
            });

            return new ChannelListDto
            {
                Items = channelDtos,
            };
        }

        public Task Update(Channel channel)
        {
            DynamoDbChannelDto channelDto = new DynamoDbChannelDto
            {
                ChannelName = channel.ChannelName,
                StreamPlatform = channel.StreamPlatform,
                DateRegistered = channel.DateRegistered,
                AvatarUrl = channel.AvatarUrl,
                ChannelUrl = channel.ChannelUrl,
            };

            return awsDynamoDbTable.PutItem(channelDto);
        }
    }
}
