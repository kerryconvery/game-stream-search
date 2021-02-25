using System.Threading.Tasks;
using GameStreamSearch.Domain.Entities;
using GameStreamSearch.Gateways;
using GameStreamSearch.Gateways.Dto.DynamoDb;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Repositories
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
            DynamoDbChannelDto channelDto = FromChannel(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }

        public async Task<Maybe<Channel>> Get(string streamPlatformName, string channelName)
        {
            var dynamoChannelDto = await awsDynamoDbTable.GetItem(streamPlatformName, channelName);

            return dynamoChannelDto.Select(c => new Channel(c.ChannelName, c.StreamPlatformName, c.DateRegistered, c.AvatarUrl, c.ChannelUrl));
        }

        public Task Update(Channel channel)
        {
            DynamoDbChannelDto channelDto = FromChannel(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }

        private DynamoDbChannelDto FromChannel(Channel channel)
        {
            return new DynamoDbChannelDto
            {
                ChannelName = channel.ChannelName,
                StreamPlatformName = channel.StreamPlatformName,
                ChannelUrl = channel.ChannelUrl,
                AvatarUrl = channel.AvatarUrl,
                DateRegistered = channel.DateRegistered,
            };
        }
    }
}
