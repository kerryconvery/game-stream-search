using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Domain.Entities;

namespace GameStreamSearch.Repositories
{
    public class ChannelRepository
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

        public async Task<Maybe<Channel>> Get(string streamPlatformName, string channelName)
        {
            var channelDto = await awsDynamoDbTable.GetItem(streamPlatformName, channelName);

            return Maybe<Channel>.ToMaybe(channelDto?.ToEntity());
        }

        public Task Update(Channel channel)
        {
            DynamoDbChannelDto channelDto = DynamoDbChannelDto.FromEntity(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }
    }
}
