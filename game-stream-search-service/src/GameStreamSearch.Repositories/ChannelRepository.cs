using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Types;
using GameStreamSearch.Repositories.Dto;
using System.Collections.Generic;

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

        public async Task<Maybe<Channel>> Get(string streamPlatformName, string channelName)
        {
            var channelDto = await awsDynamoDbTable.GetItem(streamPlatformName, channelName);

            return Maybe<Channel>.ToMaybe(channelDto?.ToEntity());
        }

        public Task Remove(string streamPlatformName, string channelName)
        {
            return awsDynamoDbTable.DeleteItem(streamPlatformName, channelName);
        }

        public async Task<IEnumerable<Channel>> GetAll()
        {
            var channelDtos = await awsDynamoDbTable.GetAllItems();

            return channelDtos.Select(c => c.ToEntity());
        }

        public Task Update(Channel channel)
        {
            DynamoDbChannelDto channelDto = DynamoDbChannelDto.FromEntity(channel);

            return awsDynamoDbTable.PutItem(channelDto);
        }
    }
}
