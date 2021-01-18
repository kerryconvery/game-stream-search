using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
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
            //DynamoDbChannelDto channelDto = DynamoDbChannelDto.FromEntity(channel);

            //return awsDynamoDbTable.PutItem(channelDto);

            return Task.FromResult<object>(null);
        }

        public async Task<Channel?> Get(StreamPlatformType streamPlatform, string channelName)
        {
            var channelDto = await awsDynamoDbTable.GetItem(streamPlatform, channelName);

            return channelDto?.ToEntity();
        }

        public Task Remove(StreamPlatformType streamPlatform, string channelName)
        {
            return awsDynamoDbTable.DeleteItem(streamPlatform, channelName);
        }

        public async Task<ChannelListDto> SelectAllChannels()
        {
            var channels = await awsDynamoDbTable.GetAllItems();

            ChannelListDto channelList = new ChannelListDto();

            var channelDtos = channels.OrderBy(c => c.DateRegistered)
                .Select(c => new ChannelDto
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
            DynamoDbChannelDto channelDto = DynamoDbChannelDto.FromEntity(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }
    }
}
