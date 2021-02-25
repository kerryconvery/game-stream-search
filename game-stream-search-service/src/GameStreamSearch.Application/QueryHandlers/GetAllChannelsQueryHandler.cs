using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Domain.Queries;
using GameStreamSearch.Gateways;
using GameStreamSearch.Gateways.Dto.DynamoDb;

namespace GameStreamSearch.Application.QueryHandlers
{
    public class GetAllChannelsQueryHandler : IQueryHandler<GetAllChannelsQuery, ChannelListDto>
    {
        private readonly AwsDynamoDbGateway<DynamoDbChannelDto> dynamoDbGateway;

        public GetAllChannelsQueryHandler(AwsDynamoDbGateway<DynamoDbChannelDto> dynamoDbGateway)
        {
            this.dynamoDbGateway = dynamoDbGateway;
        }

        public async Task<ChannelListDto> Execute(GetAllChannelsQuery query)
        {
            var channels = await dynamoDbGateway.GetAllItems();

            ChannelListDto channelList = new ChannelListDto();

            var channelDtos = channels.OrderBy(c => c.DateRegistered)
                .Select(c => new ChannelDto
                {
                    ChannelName = c.ChannelName,
                    PlatformName = c.StreamPlatformName,
                    AvatarUrl = c.AvatarUrl,
                    ChannelUrl = c.ChannelUrl,
                });

            return new ChannelListDto
            {
                Channels = channelDtos,
            };
        }
    }
}
