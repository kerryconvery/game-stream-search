using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Twitch.Mappers
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
