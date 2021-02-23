using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Models;

namespace GameStreamSearch.Application.Queries
{
    public class GetAllChannelsQuery {}

    public class GetAllChannelsQueryHandler : IQueryHandler<GetAllChannelsQuery, ChannelListDto>
    {
        private readonly IRepository<Channel> channelRepository;

        public GetAllChannelsQueryHandler(IRepository<Channel> channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        public async Task<ChannelListDto> Execute(GetAllChannelsQuery query)
        {
            var channels = await channelRepository.GetAll();

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
