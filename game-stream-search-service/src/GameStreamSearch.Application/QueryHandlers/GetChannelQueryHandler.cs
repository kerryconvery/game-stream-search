using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Domain.Queries;
using GameStreamSearch.Gateways.Dto.DynamoDb;
using GameStreamSearch.Gateways;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application.QueryHandlers
{
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
