using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Models;
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
        private IChannelRepository channelRepository;

        public GetChannelQueryHandler(IChannelRepository channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        public async Task<Maybe<ChannelDto>> Execute(GetChannelQuery query)
        {
           var channel = await channelRepository.Get(query.platformName, query.channelName);

            return channel.Select(v => ChannelDto.FromEntity(v));
        }
    }
}
