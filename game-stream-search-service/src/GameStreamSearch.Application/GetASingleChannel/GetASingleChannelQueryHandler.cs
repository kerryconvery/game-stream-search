using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Domain.Queries;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.DataAccess;
using GameStreamSearch.DataAccess.Dto;

namespace GameStreamSearch.Application.QueryHandlers
{
    public class GetChannelQueryHandler : IQueryHandler<GetChannelQuery, Maybe<ChannelDto>>
    {
        private readonly AwsDynamoDbTable<ChannelTableDto> dynamoDbGateway;

        public GetChannelQueryHandler(AwsDynamoDbTable<ChannelTableDto> dynamoDbGateway)
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
