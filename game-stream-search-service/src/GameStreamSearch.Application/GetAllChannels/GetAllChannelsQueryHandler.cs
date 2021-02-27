using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.DataAccess;
using GameStreamSearch.DataAccess.Dto;
using GameStreamSearch.Domain.Queries;

namespace GameStreamSearch.Application.QueryHandlers
{
    public class GetAllChannelsQueryHandler : IQueryHandler<GetAllChannelsQuery, ChannelListDto>
    {
        private readonly AwsDynamoDbTable<ChannelTableDto> dynamoDbGateway;

        public GetAllChannelsQueryHandler(AwsDynamoDbTable<ChannelTableDto> dynamoDbGateway)
        {
            this.dynamoDbGateway = dynamoDbGateway;
        }

        public async Task<ChannelListDto> Execute(GetAllChannelsQuery query)
        {
            var channels = await dynamoDbGateway.GetAllItems();

            ChannelListDto channelList = new ChannelListDto();

            var channelDtos = channels
                .OrderBy(c => c.DateRegistered)
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
