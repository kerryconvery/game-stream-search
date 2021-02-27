using System.Threading.Tasks;
using GameStreamSearch.DataAccess;
using GameStreamSearch.DataAccess.Dto;
using GameStreamSearch.Domain;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Repositories
{
    public class ChannelRepository
    {
        private readonly AwsDynamoDbTable<ChannelTableDto> awsDynamoDbTable;

        public ChannelRepository(AwsDynamoDbTable<ChannelTableDto> awsDynamoDbTable)
        {
            this.awsDynamoDbTable = awsDynamoDbTable;
        }

        public Task Add(Channel channel)
        {
            ChannelTableDto channelDto = FromChannel(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }

        public async Task<Maybe<Channel>> Get(string streamPlatformName, string channelName)
        {
            var dynamoChannelDto = await awsDynamoDbTable.GetItem(streamPlatformName, channelName);

            return dynamoChannelDto.Select(c => new Channel(c.ChannelName, c.StreamPlatformName, c.DateRegistered, c.AvatarUrl, c.ChannelUrl));
        }

        public Task Update(Channel channel)
        {
            ChannelTableDto channelDto = FromChannel(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }

        private ChannelTableDto FromChannel(Channel channel)
        {
            return new ChannelTableDto
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
