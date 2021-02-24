using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Queries
{
    public struct GetChannelQuery
    {
        public string platformName { get; init; }
        public string channelName { get; init; }
    }

    public class GetChannelQueryHandler : IQueryHandler<GetChannelQuery, Maybe<ChannelDto>>
    {
        private readonly AwsDynamoDbGateway<DynamoDbChannelDto> dynamoDbGateway;

        public GetChannelQueryHandler(AwsDynamoDbGateway<DynamoDbChannelDto> dynamoDbGateway)
        {
            this.dynamoDbGateway = dynamoDbGateway;
        }

        public async Task<Maybe<ChannelDto>> Execute(GetChannelQuery query)
        {
           var channel = await dynamoDbGateway.GetItem(query.platformName, query.channelName);

            return channel.Select(v => new ChannelDto
            {
                PlatformName = v.StreamPlatformName,
                ChannelName = v.ChannelName,
                ChannelUrl = v.ChannelUrl,
                AvatarUrl = v.AvatarUrl
            });
        }
    }
}
