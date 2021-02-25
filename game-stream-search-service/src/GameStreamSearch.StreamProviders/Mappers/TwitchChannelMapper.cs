using GameStreamSearch.Application.Dto;
using GameStreamSearch.Domain.Entities;
using GameStreamSearch.Gateways.Dto.Twitch.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class TwitchChannelMapper
    {
        public MaybeResult<PlatformChannelDto, StreamProviderError> Map(
            MaybeResult<TwitchChannelDto, StreamProviderError> twitchChannelResult)
        {
            return twitchChannelResult.Select(channel =>
            {
                return new PlatformChannelDto
                {
                    ChannelName = channel.display_name,
                    AvatarUrl = channel.logo,
                    ChannelUrl = channel.url,
                    StreamPlatformName = StreamPlatform.Twitch,
                };
            });
        }
    }
}
