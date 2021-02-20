using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Types;
using GameStreamSearch.Repositories.Dto;

namespace GameStreamSearch.Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly AwsDynamoDbGateway<DynamoDbChannelDto> awsDynamoDbTable;

        public ChannelRepository(AwsDynamoDbGateway<DynamoDbChannelDto> awsDynamoDbTable)
        {
            this.awsDynamoDbTable = awsDynamoDbTable;
        }

        public Task Add(Channel channel)
        {
            DynamoDbChannelDto channelDto = DynamoDbChannelDto.FromEntity(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }

        public async Task<Maybe<Channel>> Get(string streamPlatformId, string channelName)
        {
            var channelDto = await awsDynamoDbTable.GetItem(streamPlatformId, channelName);

            return Maybe<Channel>.ToMaybe(channelDto?.ToEntity());
        }

        public Task Remove(string streamPlatformId, string channelName)
        {
            return awsDynamoDbTable.DeleteItem(streamPlatformId, channelName);
        }

        public async Task<ChannelListDto> SelectAllChannels()
        {
            var channels = await awsDynamoDbTable.GetAllItems();

            ChannelListDto channelList = new ChannelListDto();

            var channelDtos = channels.OrderBy(c => c.DateRegistered)
                .Select(c => new ChannelDto
                {
                    ChannelName = c.ChannelName,
                    StreamPlatformId = c.StreamPlatformId,
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
